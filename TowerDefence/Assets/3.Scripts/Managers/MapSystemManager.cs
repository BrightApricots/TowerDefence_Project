using UnityEngine;

public class MapSystemManager : MonoBehaviour
{
    private static MapSystemManager instance;
    public static MapSystemManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("MapSystemManager");
                instance = go.AddComponent<MapSystemManager>();
                DontDestroyOnLoad(go);
                instance.Initialize();
            }
            return instance;
        }
    }

    // 필요한 시스템 컴포넌트들
    public PlacementSystem PlacementSystem { get; private set; }
    public PathManager PathManager { get; private set; }
    public AGrid Grid { get; private set; }
    public PathFinding PathFinding { get; private set; }
    public PathRequestManager PathRequestManager { get; private set; }
    public PreviewSystem PreviewSystem { get; private set; }
    public InputManager InputManager { get; private set; }

    [SerializeField] private ObjectsDatabaseSO database;

    private void Initialize()
    {
        // 각 시스템을 위한 게임오브젝트 생성 및 컴포넌트 추가
        var placementGO = new GameObject("PlacementSystem");
        PlacementSystem = placementGO.AddComponent<PlacementSystem>();
        placementGO.transform.SetParent(transform);

        var pathManagerGO = new GameObject("PathManager");
        PathManager = pathManagerGO.AddComponent<PathManager>();
        pathManagerGO.transform.SetParent(transform);

        var gridGO = new GameObject("Grid");
        Grid = gridGO.AddComponent<AGrid>();
        gridGO.transform.SetParent(transform);

        var pathFindingGO = new GameObject("PathFinding");
        PathFinding = pathFindingGO.AddComponent<PathFinding>();
        PathRequestManager = pathFindingGO.AddComponent<PathRequestManager>();
        pathFindingGO.transform.SetParent(transform);

        var previewGO = new GameObject("PreviewSystem");
        PreviewSystem = previewGO.AddComponent<PreviewSystem>();
        previewGO.transform.SetParent(transform);

        var inputGO = new GameObject("InputManager");
        InputManager = inputGO.AddComponent<InputManager>();
        inputGO.transform.SetParent(transform);

        // 시스템 간의 참조 설정
        PlacementSystem.Initialize(InputManager, Grid, database, PreviewSystem);
        PathManager.Initialize(Grid);
        Grid.Initialize();
        PathFinding.Initialize(Grid, PathRequestManager);
        PreviewSystem.Initialize();
    }

    // 씬 전환 시 맵 데이터 초기화
    public void ResetMapData()
    {
        Grid.UpdateGrid();
        PathManager.UpdateAllPaths();
        ObjectPlacer.Instance.Clear();
        GridData.Instance.Clear();
    }

    // 씬 전환 시 호출될 메서드
    public void SetupNewMap(MapData mapData)
    {
        ResetMapData();
        // 새로운 맵 데이터로 초기화
        Grid.SetupGrid(mapData.gridSize, mapData.unwalkableMask);
        // 기타 필요한 초기화...
    }
} 