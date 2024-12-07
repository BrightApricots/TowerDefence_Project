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
    public static PlacementSystem Instance {  get { return instance; } }

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        gridVisualization.SetActive(false);
        BlockData = new();
        TowerData = new();
    }

    public void StartPlacement(int ID)
    {
        //StopPlacement();
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

        // 임시 노드 상�� 설정
        placementState.SetTemporaryNodes(gridPosition, false);

        PathManager.Instance.UpdatePreviewPathWithDelay((isValid) => {
            pathValid = isValid;
            checkComplete = true;
        });

        float timeout = Time.time + 0.5f;
        while (!checkComplete && Time.time < timeout)
        {
            // 대기 중에도 �속 유효성 검사
            if (!placementState.IsValidPlacement(gridPosition))
            {
                placementState.RestoreTemporaryNodes();
                isPlacing = false;
                yield break;
            }
            yield return null;
        }

        // 노드 상태 �원
        placementState.RestoreTemporaryNodes();

        // 최종 유효� 검사
        if (pathValid && placementState.IsValidPlacement(gridPosition))
        {
            placementState.OnAction(gridPosition);
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
}
