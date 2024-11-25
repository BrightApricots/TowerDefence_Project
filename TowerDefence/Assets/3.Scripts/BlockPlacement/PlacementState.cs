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
            // 타워 로직은 그대로 유지
            Vector3Int positionBelow = new Vector3Int(gridPosition.x, 0, gridPosition.z);
            if (BlockData.GetRepresentationIndex(positionBelow) != -1)
            {
                floor = 1;
                validity = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);
            }
        }
        else
        {
            validity = BlockData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor) &&
                TowerData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);

            if (validity && PathManager.Instance != null && PathManager.Instance.HasBothPoints())
            {
                // 시작점과 목표점 체크
                Vector3Int spawnGridPos = grid.WorldToCell(PathManager.Instance.GetSpawnPosition());
                Vector3Int targetGridPos = grid.WorldToCell(PathManager.Instance.GetTargetPosition());
                
                foreach (var cell in database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation))
                {
                    Vector3Int blockPos = new Vector3Int(
                        gridPosition.x + cell.x,
                        gridPosition.y,
                        gridPosition.z + cell.y
                    );
                    
                    if (blockPos.x == spawnGridPos.x && blockPos.z == spawnGridPos.z ||
                        blockPos.x == targetGridPos.x && blockPos.z == targetGridPos.z)
                    {
                        validity = false;
                        break;
                    }
                }

                if (validity)
                {
                    Dictionary<ANode, bool> originalStates = new Dictionary<ANode, bool>();
                    List<ANode> modifiedNodes = new List<ANode>();

                    try
                    {
                        // 임시로 노드들을 막음
                        foreach (var cell in database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation))
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
                                modifiedNodes.Add(node);
                            }
                        }

                        // 프리뷰 경로 계산
                        PathManager.Instance.UpdatePreviewPath();
                        validity = PathManager.Instance.HasValidPath;
                    }
                    finally
                    {
                        // 노드 상태 복원
                        foreach (var node in modifiedNodes)
                        {
                            if (originalStates.ContainsKey(node))
                            {
                                node.walkable = originalStates[node];
                            }
                        }
                    }
                }
            }
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
            
            if (BlockData.GetRepresentationIndex(positionBelow) != -1)
            {
                floor = 1;
                canPlace = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);
            }
        }
        else
        {
            canPlace = BlockData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor) &&
                TowerData.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), floor);

            if (canPlace && PathManager.Instance != null && PathManager.Instance.HasBothPoints())
            {
                // 경로 체크는 한 번만 수행
                canPlace = CheckPathValidity(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation));
            }
        }

        if (canPlace)
        {
            GridData selectedData = database.IsBlock(database.objectsData[selectedObjectIndex].ID) ? BlockData : TowerData;
             
            Vector3 position = grid.CellToWorld(gridPosition);
            position += new Vector3(0.5f, floor, 0.5f);

            int index = objectPlacer.PlaceObject(
                database.objectsData[selectedObjectIndex].Prefab,
                position,
                Quaternion.Euler(0, 90 * currentRotation, 0));

            selectedData.AddObjectAt(
                gridPosition,
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation),
                database.objectsData[selectedObjectIndex].ID,
                index,
                floor);

            if (database.IsBlock(database.objectsData[selectedObjectIndex].ID))
            {
                var cells = database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation);
                foreach (var cell in cells)
                {
                    Vector3Int blockPos = new Vector3Int(
                        gridPosition.x + cell.x,
                        gridPosition.y,
                        gridPosition.z + cell.y
                    );
                    aGrid.UpdateNode(blockPos, true);
                }
                PathManager.Instance.UpdatePath();
            }
        }
    }
}