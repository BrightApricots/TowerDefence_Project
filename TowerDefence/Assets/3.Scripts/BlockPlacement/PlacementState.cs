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

    private bool CheckPathValidity(Vector3Int gridPosition, List<Vector2Int> cells)
    {
        if (!PathManager.Instance.HasValidPath)
            return false;

        // 시작점과 목표점의 그리드 좌표로 변환
        Vector3Int spawnGridPos = grid.WorldToCell(PathManager.Instance.GetSpawnPosition());
        Vector3Int targetGridPos = grid.WorldToCell(PathManager.Instance.GetTargetPosition());
        
        // 시작점/목표점 체크
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            
            if (blockPos.x == spawnGridPos.x && blockPos.z == spawnGridPos.z ||
                blockPos.x == targetGridPos.x && blockPos.z == targetGridPos.z)
            {
                return false;
            }
        }

        Dictionary<ANode, bool> originalStates = new Dictionary<ANode, bool>();
        
        // 임시로 노드들을 막음
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            ANode node = aGrid.ANodeFromWorldPoint(new Vector3(blockPos.x, 0, blockPos.z));
            if (node != null && !originalStates.ContainsKey(node))
            {
                originalStates[node] = node.walkable;
                node.walkable = false;
            }
        }

        // 경로 체크
        PathManager.Instance.UpdatePreviewPath();  // 프리뷰 경로 사용
        bool isValid = PathManager.Instance.HasValidPath;

        // 노드 상태 복원
        foreach (var pair in originalStates)
        {
            pair.Key.walkable = pair.Value;
        }
        PathManager.Instance.UpdatePath();

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
            // 블록 설치 로직은 그대로 유지
            bool hasObstacle = false;
            Vector3 worldPosition = grid.CellToWorld(gridPosition);
            foreach (var cell in database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation))
            {
                Vector3 checkPosition = worldPosition + new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f);
                if (Physics.CheckSphere(checkPosition, 0.3f, aGrid.unwalkableMask))
                {
                    hasObstacle = true;
                    break;
                }
            }

            if (hasObstacle)
            {
                preview.UpdatePosition(grid.CellToWorld(gridPosition), false, floor);
                return;
            }

            validity = BlockData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor) &&
                TowerData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);
        }

        preview.UpdatePosition(grid.CellToWorld(gridPosition), validity, floor);
    }

    public void OnAction(Vector3Int gridPosition)
    {
        int floor = 0;
        bool canPlace = false;

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
                canPlace = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);
                Debug.Log($"Tower placement - Block below: {hasBlockBelow}, Obstacle below: {hasObstacleBelow}, Can place: {canPlace}");
            }
        }
        else
        {
            // 블록인 경우 기존 장애물 체크 유지
            bool hasObstacle = false;
            Vector3 worldPosition = grid.CellToWorld(gridPosition);
            foreach (var cell in database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation))
            {
                Vector3 checkPosition = worldPosition + new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f);
                if (Physics.CheckSphere(checkPosition, 0.3f, aGrid.unwalkableMask))
                {
                    hasObstacle = true;
                    break;
                }
            }

            if (hasObstacle)
            {
                Debug.Log("Cannot place block - obstacle detected");
                return;
            }

            canPlace = BlockData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor) &&
                TowerData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);

            if (canPlace && PathManager.Instance != null && PathManager.Instance.HasBothPoints())
            {
                canPlace = CheckPathValidity(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation));
            }
        }

        if (canPlace)
        {
            GridData selectedData = database.IsBlock(database.objectsData[selectedObjectIndex].ID) ? BlockData : TowerData;
            
            Vector3 position = grid.CellToWorld(gridPosition);
            position += new Vector3(0.5f, floor, 0.5f);

            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, position, 
                Quaternion.Euler(0, 90 * currentRotation, 0));

            selectedData.AddObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation),
                database.objectsData[selectedObjectIndex].ID, index, floor);

            Debug.Log($"Object placed at position: {position}, floor: {floor}, index: {index}");
        }
    }
}