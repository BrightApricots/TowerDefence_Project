using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[System.Serializable]
public class MonsterSpawnData
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public int spawnCount; // 몬스터 등장 수량
}

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private List<MonsterSpawnData> monsterList = new List<MonsterSpawnData>(); // 몬스터 리스트
    [SerializeField]
    private float spawnInterval = 0.5f; // 몬스터 생성 주기

    private bool isSpawning = false; // 몬스터 생성 활성화 여부
    private int currentSpawnIndex = 0; // 현재 스폰 지점 인덱스
    private Dictionary<GameObject, int> remainingSpawnCount = new Dictionary<GameObject, int>(); // 남은 몬스터 수량
    private List<GameObject> weightedMonsterPool = new List<GameObject>(); // 등장 확률에 따라 가중치를 부여한 몬스터 리스트

    public event System.Action OnSpawnComplete; // 모든 몬스터 스폰 완료 이벤트
    public event System.Action<GameObject> OnMonsterSpawned; // 몬스터 생성 이벤트
    public event System.Action<GameObject> OnMonsterDestroyed; // 몬스터 파괴 이벤트

    void Start()
    {
        InitializeSpawnCounts(); // 몬스터 수량 초기화
        CreateWeightedMonsterPool(); // 확률 기반 가중치 풀 생성
    }

    private void InitializeSpawnCounts()
    {
        remainingSpawnCount.Clear();

        // 몬스터별 등장 수량 초기화
        foreach (var spawnData in monsterList)
        {
            if (spawnData.monsterPrefab != null)
            {
                remainingSpawnCount[spawnData.monsterPrefab] = spawnData.spawnCount;
            }
        }
    }

    private void CreateWeightedMonsterPool()
    {
        weightedMonsterPool.Clear();

        // 몬스터별 등장 확률에 따라 가중치 기반 풀 생성
        foreach (var spawnData in monsterList)
        {
            if (spawnData.monsterPrefab != null && spawnData.spawnCount > 0)
            {
                for (int i = 0; i < spawnData.spawnCount; i++)
                {
                    weightedMonsterPool.Add(spawnData.monsterPrefab);
                }
            }
        }
    }

    private void SpawnMonster()
    {
        var spawnPoints = PathManager.Instance.GetSpawnPoints();
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.Log("스폰 지점이 없습니다.");
            return;
        }

        // 유효한 스폰 지점 필터링
        var validSpawnPoints = spawnPoints.Where(sp =>
        {
            if (sp == null) return false;
            var path = PathManager.Instance.GetCurrentPath(sp);
            return path != null && path.Length > 0;
        }).ToList();

        if (validSpawnPoints.Count == 0)
        {
            Debug.Log("유효한 스폰 지점이 없습니다.");
            return;
        }

        // 순환 방식으로 스폰 지점 선택
        currentSpawnIndex = (currentSpawnIndex + 1) % validSpawnPoints.Count;
        Transform selectedSpawnPoint = validSpawnPoints[currentSpawnIndex];

        // 등장 확률 기반으로 몬스터 종류 선택
        if (weightedMonsterPool.Count == 0)
        {
            Debug.Log("모든 몬스터 등장 완료.");
            StopSpawning();
            return;
        }

        GameObject selectedMonster = weightedMonsterPool[Random.Range(0, weightedMonsterPool.Count)];

        if (remainingSpawnCount.ContainsKey(selectedMonster))
        {
            remainingSpawnCount[selectedMonster]--;
            if (remainingSpawnCount[selectedMonster] <= 0)
            {
                weightedMonsterPool.RemoveAll(m => m == selectedMonster);
            }
        }

        Vector3 spawnPosition = PathManager.Instance.GetSpawnPosition(selectedSpawnPoint);
        GameObject monsterObj = Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();

        if (monster != null)
        {
            monster.Initialize(selectedSpawnPoint);
            monster.OnDestroyed += () => OnMonsterDestroyed?.Invoke(monsterObj);
        }

        OnMonsterSpawned?.Invoke(monsterObj);
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            if (PathManager.Instance != null && PathManager.Instance.HasValidPath)
            {
                SpawnMonster();
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        OnSpawnComplete?.Invoke();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            currentSpawnIndex = -1;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    // 새로운 웨이브 데이터를 설정하기 위한 메서드 추가
    public void SetMonsterData(List<MonsterSpawnData> newMonsterList)
    {
        monsterList = newMonsterList;
        InitializeSpawnCounts();
        CreateWeightedMonsterPool();
    }
}

// 중간 완료