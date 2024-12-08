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
    public AGrid AGrid { get; private set; }  // Grid를 AGrid로 변경
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

        var gridGO = new GameObject("AGrid");
        AGrid = gridGO.AddComponent<AGrid>();  // Grid를 AGrid로 변경
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
        //PlacementSystem.Initialize(InputManager, AGrid, database, PreviewSystem);  // Grid를 AGrid로 변경
        //PathManager.Initialize(AGrid);  // Grid를 AGrid로 변경
        //AGrid.Initialize();  // Grid를 AGrid로 변경
        //PathFinding.Initialize(AGrid, PathRequestManager);  // Grid를 AGrid로 변경
        //PreviewSystem.Initialize();
    }

    public void ResetMapData()
    {
        // 순서 중요: GridData를 먼저 정리하고 나머지 처리
        GridData.Instance.Clear();
        ObjectPlacer.Instance.Clear();
        
        if (AGrid != null)  // Grid를 AGrid로 변경
        {
            AGrid.UpdateGrid();
        }
        
        if (PathManager != null)
        {
            PathManager.UpdateAllPaths();
        }
    }
}