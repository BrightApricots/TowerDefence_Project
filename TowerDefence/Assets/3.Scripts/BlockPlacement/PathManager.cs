using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathManager : MonoBehaviour
{
    private static PathManager instance;
    public static PathManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PathManager>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private List<Transform> spawnPoints;  // 여러 스폰 포인트
    [SerializeField]
    private Transform targetPoint;
    private Dictionary<Transform, AUnit> pathUnits = new Dictionary<Transform, AUnit>();
    
    private bool isUpdating = false;
    private int pendingUpdates = 0;

    // 경로 관리를 위한 추가 속성들
    private Dictionary<Transform, PathData> pathDataMap = new Dictionary<Transform, PathData>();

    private class PathData
    {
        public Vector3[] CurrentPath { get; set; }
        public bool IsValid { get; set; }
        public AUnit PathUnit { get; set; }
    }

    // 경로 업데이트 이벤트
    public event System.Action<Transform, Vector3[]> OnPathUpdated;
    public event System.Action<bool> OnValidityChanged;

    private void NotifyPathUpdate(Transform spawnPoint, Vector3[] path)
    {
        OnPathUpdated?.Invoke(spawnPoint, path);
    }

    private void NotifyValidityChange(bool isValid)
    {
        OnValidityChanged?.Invoke(isValid);
    }

    [SerializeField]
    private Material pathLineMaterial;  // Inspector에서 할당할 Material

    private void Start()
    {
        if (pathLineMaterial == null)
        {
            pathLineMaterial = new Material(Shader.Find("Sprites/Default"));
            pathLineMaterial.color = Color.white;
        }

        // pathUnits와 pathDataMap 초기화
        pathUnits = new Dictionary<Transform, AUnit>();
        pathDataMap = new Dictionary<Transform, PathData>();

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint == null) continue;

            GameObject unitObj = new GameObject($"PathUnit_{spawnPoint.name}");
            AUnit unit = unitObj.AddComponent<AUnit>();
            
            // Material 설정
            unit.SetLineMaterial(pathLineMaterial);
            
            // 위치와 타겟 설정
            unit.transform.position = spawnPoint.position;
            unit.SetTarget(targetPoint);

            // PathData 생성 및 저장
            PathData pathData = new PathData
            {
                PathUnit = unit,
                IsValid = false,
                CurrentPath = null
            };
            
            pathUnits.Add(spawnPoint, unit);
            pathDataMap.Add(spawnPoint, pathData);

            // 경로 업데이트 이벤트 구독
            unit.OnPathUpdated += (newPath, success) =>
            {
                if (success && newPath != null)
                {
                    pathData.CurrentPath = newPath;
                    pathData.IsValid = true;
                    NotifyPathUpdate(spawnPoint, newPath);
                }
            };
        }
        
        UpdateAllPaths();
    }

    public Vector3[] GetCurrentPath(Transform spawnPoint = null)
    {
        if (spawnPoint == null)
            spawnPoint = GetBestSpawnPoint();

        if (pathDataMap.TryGetValue(spawnPoint, out PathData data))
        {
            if (data.CurrentPath == null || data.CurrentPath.Length == 0)
            {
                // AUnit에서 직접 경로 가져오기 시도
                AUnit unit = pathUnits[spawnPoint];
                if (unit != null)
                {
                    return unit.GetCurrentPath();
                }
            }
            return data.CurrentPath;
        }
        
        Debug.LogWarning($"No path data found for spawn point: {spawnPoint.name}");
        return null;
    }

    public void UpdateAllPaths()
    {
        UpdatePath();
    }

    public void CheckPreviewPath()
    {
        // 모든 PathData의 IsValid를 초기화
        foreach (var data in pathDataMap.Values)
        {
            data.IsValid = false;
            data.CurrentPath = null;
        }

        // 모든 유닛의 프리뷰 초기화 및 활성화
        foreach (var kvp in pathUnits)
        {
            if (kvp.Value != null)
            {
                kvp.Value.ShowPreviewPath(true);
                kvp.Value.ShowActualPath(true);
                kvp.Value.CheckPreviewPath();  // 각 유닛별로 프리뷰 경로 계산
            }
        }
    }

    private IEnumerator WaitForAllPathsCalculated()
    {
        yield return new WaitForEndOfFrame();

        // 모든 경로의 유효성 확인
        bool anyValidPath = false;
        foreach (var data in pathDataMap.Values)
        {
            if (data.IsValid && data.CurrentPath != null)
            {
                anyValidPath = true;
                // break 제거하여 모든 경로 확인
            }
        }

        HasValidPath = anyValidPath;
        NotifyValidityChange(anyValidPath);
    }

    public void UpdatePath()
    {
        if (isUpdating)
        {
            pendingUpdates++;
            return;
        }

        isUpdating = true;

        foreach (var kvp in pathUnits)
        {
            if (kvp.Value != null)
            {
                kvp.Value.UpdatePath();
            }
        }

        // 타임아웃 설정
        StartCoroutine(UpdateTimeout());
    }

    private IEnumerator UpdateTimeout()
    {
        yield return new WaitForSeconds(1f);
        if (isUpdating)
        {
            isUpdating = false;
            if (pendingUpdates > 0)
            {
                pendingUpdates--;
                UpdatePath();
            }
        }
    }

    // 가장 짧은 경로를 가진 스폰 포인트 선택
    public Transform GetBestSpawnPoint()
    {
        Transform bestSpawn = null;
        float shortestPath = float.MaxValue;

        foreach (var kvp in pathUnits)
        {
            Vector3[] path = kvp.Value.GetCurrentPath();
            if (path != null)
            {
                float pathLength = CalculatePathLength(path);
                if (pathLength < shortestPath)
                {
                    shortestPath = pathLength;
                    bestSpawn = kvp.Key;
                }
            }
        }

        return bestSpawn;
    }

    private float CalculatePathLength(Vector3[] path)
    {
        float length = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            length += Vector3.Distance(path[i], path[i + 1]);
        }
        return length;
    }

    private void OnDestroy()
    {
        foreach (var unit in pathUnits.Values)
        {
            if (unit != null)
            {
                Destroy(unit.gameObject);
            }
        }
        pathUnits.Clear();
    }

    public void CleanupUnusedPaths()
    {
        List<Transform> keysToRemove = new List<Transform>();
        foreach (var kvp in pathUnits)
        {
            if (kvp.Key == null || kvp.Value == null)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            if (pathUnits[key] != null)
            {
                Destroy(pathUnits[key].gameObject);
            }
            pathUnits.Remove(key);
        }
    }

    public List<Transform> GetSpawnPoints() => spawnPoints;
    public Vector3 GetTargetPosition() => targetPoint.position;
    public Vector3 GetSpawnPosition(Transform spawnPoint = null)
    {
        if (spawnPoint == null)
            spawnPoint = GetBestSpawnPoint();
            
        return spawnPoint != null ? spawnPoint.position : spawnPoints[0].position;
    }
    public bool HasBothPoints() => spawnPoints.Count > 0 && targetPoint != null;
    public bool HasValidPath { get; private set; }
    public void OnPathCalculated(Vector3[] path, bool success, bool isPreview = false)
    {
        if (isPreview)
        {
            if (success && path != null)
            {
                foreach (var kvp in pathDataMap)
                {
                    float distance = Vector3.Distance(kvp.Value.PathUnit.transform.position, path[0]);
                    if (distance < 0.1f)
                    {
                        kvp.Value.IsValid = true;
                        kvp.Value.CurrentPath = path;
                        NotifyPathUpdate(kvp.Key, path);
                    }
                }
            }
            HasValidPath = pathDataMap.Values.Any(data => data.IsValid);
        }
        else
        {
            // 실제 경로 처리는 그대로 유지
            HasValidPath = success;
            if (success && path != null)
            {
                foreach (var kvp in pathDataMap)
                {
                    float distance = Vector3.Distance(kvp.Value.PathUnit.transform.position, path[0]);
                    if (distance < 0.1f)
                    {
                        kvp.Value.CurrentPath = path;
                        kvp.Value.IsValid = true;
                        NotifyPathUpdate(kvp.Key, path);
                    }
                }
            }
            NotifyValidityChange(success);
        }
    }

    public void UpdatePreviewPathWithDelay(System.Action<bool> callback)
    {
        StartCoroutine(UpdatePreviewPathCoroutine(callback));
    }

    private IEnumerator UpdatePreviewPathCoroutine(System.Action<bool> callback)
    {
        yield return new WaitForEndOfFrame();
        CheckPreviewPath();
        callback?.Invoke(HasValidPath);
    }
} 