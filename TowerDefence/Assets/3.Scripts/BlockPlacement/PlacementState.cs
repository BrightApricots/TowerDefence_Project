using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int currentRotation = 0;
    Grid grid;
    PreviewSystem preview;
    ObjectsDatabaseSO database;
    GridData BlockData;
    GridData TowerData;
    ObjectPlacer objectPlacer;
    InputManager inputManager;
    AGrid aGrid;
    private LayerMask towerPlaceableLayer;

    public PlacementState(int ID, Grid grid, PreviewSystem preview, ObjectsDatabaseSO database, GridData blockData,
            GridData towerData, ObjectPlacer objectPlacer, InputManager inputManager, AGrid aGrid)
    {
        this.grid = grid;
        this.preview = preview;
        this.database = database;
        this.BlockData = blockData;
        this.TowerData = towerData;
        this.objectPlacer = objectPlacer;
        this.inputManager = inputManager;
        this.aGrid = aGrid;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            preview.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation),
                currentRotation);
        }

        inputManager.OnRotate += RotateStructure;

        towerPlaceableLayer = LayerMask.GetMask("TowerPlaceable");
    }

    private void RotateStructure()
    {
        if (!inputManager.IsPointerOverUI())
        {
            currentRotation = (currentRotation + 1) % 4;
            preview.UpdateRotation(currentRotation);
        }
    }

    public void EndState()
    {
        preview.StopShowingPreview();
        inputManager.OnRotate -= RotateStructure;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, List<Vector2Int> cells, bool isPreview = false)
    {
        // 시작 위치와 종료 위치 체크
        Vector3Int targetGridPos = grid.WorldToCell(PathManager.Instance.GetTargetPosition());
        foreach (var spawnPoint in PathManager.Instance.GetSpawnPoints())
        {
            Vector3Int spawnGridPos = grid.WorldToCell(spawnPoint.position);
            
            foreach (var cell in cells)
            {
                Vector3Int blockPos = new Vector3Int(
                    gridPosition.x + cell.x,
                    gridPosition.y,
                    gridPosition.z + cell.y
                );
                
                // 시작 위치나 종료 위치에 블록을 설치하려고 하는 경우
                if ((blockPos.x == spawnGridPos.x && blockPos.z == spawnGridPos.z) ||
                    (blockPos.x == targetGridPos.x && blockPos.z == targetGridPos.z))
                {
                    if (isPreview) Debug.Log("Cannot place block on spawn or target position");
                    return false;
                }
            }
        }

        // 현재 위치에 이미 블록이 있는지 확인
        if (!BlockData.CanPlaceObjectAt(gridPosition, cells, 0) ||
            !TowerData.CanPlaceObjectAt(gridPosition, cells, 0))
        {
            return false;
        }

        // 장애물 체크 (프리뷰와 템플릿 제외)
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        foreach (var cell in cells)
        {
            Vector3 checkPosition = worldPosition + new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f);
            Collider[] hitColliders = Physics.OverlapSphere(checkPosition, 0.3f, aGrid.unwalkableMask);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider != null)
                {
                    GameObject obj = hitCollider.gameObject;
                    if (!obj.name.Contains("Preview") && 
                        !obj.name.Contains("Template"))
                    {
                        return false;
                    }
                }
            }
        }

        // 임로 노드들을 막고 프리뷰 경로로 유효성 확인
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            aGrid.SetTemporaryNodeState(blockPos, false);
        }

        // 프리뷰 경로 업데이트하여 경로 차단 여부 확인
        PathManager.Instance.CheckPreviewPath();
        bool isValid = PathManager.Instance.HasValidPath;

        // 노드 상태 복원
        aGrid.RestoreTemporaryNodes();

        if (!isValid && isPreview)
        {
            Debug.Log("Block placement would block all possible paths");
        }

        return isValid;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        int floor = 0;
        bool validity = false;
        
        if (database.IsTower(database.objectsData[selectedObjectIndex].ID))
        {
            Vector3Int positionBelow = new Vector3Int(gridPosition.x, 0, gridPosition.z);
            
            // 일반 블록과 장애물 모두 체크
            bool hasBlockBelow = BlockData.GetRepresentationIndex(positionBelow) != -1;
            bool hasObstacleBelow = false;

            // 장애물 체크
            Vector3 checkPosition = grid.CellToWorld(positionBelow) + new Vector3(0.5f, 0, 0.5f);
            if (Physics.CheckSphere(checkPosition, 0.3f, aGrid.unwalkableMask))
            {
                hasObstacleBelow = true;
            }

            if (hasBlockBelow || hasObstacleBelow)
            {
                floor = 1;
                validity = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);
                Debug.Log($"Tower preview - Block below: {hasBlockBelow}, Obstacle below: {hasObstacleBelow}, Can place: {validity}");
            }
        }
        else
        {
            validity = CheckPlacementValidity(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                true);  // isPreview = true
        }

        preview.UpdatePosition(grid.CellToWorld(gridPosition), validity, floor);
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        int floor = 0;
        bool canPlace = false;
        int currentID = database.objectsData[selectedObjectIndex].ID;
        bool canPlaceTower = false;

        if (database.IsTower(currentID))
        {
            Vector3Int positionBelow = new Vector3Int(gridPosition.x, 0, gridPosition.z);
            bool hasBlockBelow = BlockData.GetRepresentationIndex(positionBelow) != -1;
            bool hasObstacleBelow = false;

            // 장애물 체크
            Vector3 checkPosition = grid.CellToWorld(positionBelow) + new Vector3(0.5f, 0, 0.5f);
            
            // 장애물이 있는지 체크하고, 있다면 그 장애물이 타워 설치 가능한지 확인
            Collider[] obstacles = Physics.OverlapSphere(checkPosition, 0.3f, aGrid.unwalkableMask);
            foreach (var obstacle in obstacles)
            {
                hasObstacleBelow = true;
                if (obstacle.CompareTag("TowerPlaceable"))
                {
                    canPlaceTower = true;
                    break;  // 타워 설치 가능한 장애물을 찾았으면 더 이상 검사할 필요 없음
                }
            }

            if ((hasBlockBelow || (hasObstacleBelow && canPlaceTower)))
            {
                floor = 1;
                canPlace = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                    floor);
                Debug.Log($"Tower placement - Block below: {hasBlockBelow}, Obstacle below: {hasObstacleBelow}, Can place tower: {canPlaceTower}");
            }
        }
        else
        {
            canPlace = CheckPlacementValidity(gridPosition, database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), false);  
        }

        if (canPlace)
        {
            Debug.Log($"Attempting to place {(database.IsTower(currentID) ? "tower" : "block")} ID: {currentID}");
            GridData selectedData = database.IsBlock(currentID) ? BlockData : TowerData;
            
            Vector3 position = grid.CellToWorld(gridPosition);
            position += new Vector3(0.5f, floor, 0.5f);

            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, position, Quaternion.Euler(0, 90 * currentRotation, 0));
            selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation),currentID, index, floor);

            if (!database.IsTower(currentID))
            {
                // AGrid 업데이트
                foreach (var cell in database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation))
                {
                    Vector3Int blockPos = new Vector3Int(gridPosition.x + cell.x, gridPosition.y, gridPosition.z + cell.y);
                    aGrid.UpdateNode(blockPos, true);
                }

                // 경로 재계산
                if (PathManager.Instance != null)
                {
                    PathManager.Instance.UpdateAllPaths();
                    Debug.Log($"Path validity after placing block {currentID}: {PathManager.Instance.HasValidPath}");
                }
            }
        }
        else
        {
            Debug.Log($"Cannot place {(database.IsTower(currentID) ? "tower" : "block")} ID {currentID} - placement conditions not met");
        }
    }
}
