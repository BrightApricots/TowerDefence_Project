using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementSystem : MonoBehaviour
{
    private static PlacementSystem instance;
    public static PlacementSystem Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에서 기존 인스턴스 찾기
                instance = FindObjectOfType<PlacementSystem>();
                
                // 없으면 새로 생성
                if (instance == null)
                {
                    GameObject go = new GameObject("PlacementSystem");
                    instance = go.AddComponent<PlacementSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private AGrid aGrid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData BlockData, TowerData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

    private bool isPlacing = false;

    // 블록 배치 성공 시 발생하는 이벤트
    public event System.Action<int, string> OnPlacementSuccess;
    private string currentCardID;  // 현재 선택된 카드의 ID

    private void Awake()
    {
        if (instance == null)
        {
            // 새로운 루트 게임오브젝트 생성
            GameObject newGO = new GameObject("PlacementSystem");
            
            // 현재 컴포넌트의 모든 참조와 값을 새 오브젝트로 복사
            PlacementSystem newSystem = newGO.AddComponent<PlacementSystem>();
            newSystem.inputManager = this.inputManager;
            newSystem.grid = this.grid;
            newSystem.aGrid = this.aGrid;
            newSystem.database = this.database;
            newSystem.gridVisualization = this.gridVisualization;
            newSystem.preview = this.preview;
            newSystem.objectPlacer = this.objectPlacer;
            
            // 새 오브젝트를 DontDestroyOnLoad로 설정
            DontDestroyOnLoad(newGO);
            
            instance = newSystem;
            
            // 원래 오브젝트 제거
            Destroy(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        BlockData = new();
        TowerData = new();
    }

    private void Start()
    {
        gridVisualization.SetActive(false);
        BlockData = new();
        TowerData = new();
    }

    public void StartPlacement(int ID, string cardID)
    {
        currentCardID = cardID;  // 카드 ID 저장
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, preview, database, BlockData, TowerData, objectPlacer, inputManager, aGrid);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnClicked += StopPlacement;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(
            grid, 
            preview, 
            BlockData, 
            TowerData, 
            objectPlacer, 
            aGrid,
            inputManager);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI() || isPlacing)
        {
            return;
        }
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition); 

        if (buildingState is PlacementState placementState)
        {
            StartCoroutine(HandlePlacement(gridPosition, placementState));
        }
        else
        {
            buildingState.OnAction(gridPosition);
        }
    }

    private IEnumerator HandlePlacement(Vector3Int gridPosition, PlacementState placementState)
    {
        isPlacing = true;
        
        // 경로 체크 전에 한번 더 유효성 검사
        if (!placementState.IsValidPlacement(gridPosition))
        {
            isPlacing = false;
            yield break;
        }

        bool pathValid = false;
        bool checkComplete = false;

        // 임시 노드 상태 설정
        placementState.SetTemporaryNodes(gridPosition, false);

        PathManager.Instance.UpdatePreviewPathWithDelay((isValid) => {
            pathValid = isValid;
            checkComplete = true;
        });

        float timeout = Time.time + 0.5f;
        while (!checkComplete && Time.time < timeout)
        {
            // 대기 중에도 지속 유효성 ���사
            if (!placementState.IsValidPlacement(gridPosition))
            {
                placementState.RestoreTemporaryNodes();
                isPlacing = false;
                yield break;
            }
            yield return null;
        }

        // 노드 상태 원복
        placementState.RestoreTemporaryNodes();

        // 최종 유효성 검사
        if (pathValid && placementState.IsValidPlacement(gridPosition))
        {
            placementState.OnAction(gridPosition);
            // 배치 성공 시 이벤트에 카드 ID도 함께 전달
            OnPlacementSuccess?.Invoke(placementState.GetCurrentID(), currentCardID);
        }

        isPlacing = false;
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
    //        floorData : 
    //        furnitureData;

    //    return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnClicked -= StopPlacement;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    public void RequestPathUpdate()
    {
        StartCoroutine(UpdatePathsAfterDelay());
    }

    private IEnumerator UpdatePathsAfterDelay()
    {
        // 노드 상태가 완전히 업데이트될 때까지 잠시 대기
        yield return new WaitForEndOfFrame();

        // 경로 업데이트
        if (PathManager.Instance != null)
        {
            PathManager.Instance.UpdateAllPaths();
        }
    }

    // 타워 배치 메서드
    public void StartTowerPlacement(int ID)
    {
        if (!database.IsTower(ID))
        {
            Debug.LogError($"Attempted to place non-tower object (ID: {ID}) using StartTowerPlacement");
            return;
        }

        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, preview, database, BlockData, TowerData, objectPlacer, inputManager, aGrid);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnClicked += StopPlacement;
        inputManager.OnExit += StopPlacement;
    }

    public void Initialize(InputManager inputManager, AGrid grid, ObjectsDatabaseSO database, PreviewSystem preview)
    {
        this.inputManager = inputManager;
        this.grid = grid;
        this.database = database;
        this.preview = preview;
        
        BlockData = new GridData();
        TowerData = new GridData();
    }
}
