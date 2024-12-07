using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

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
    private bool isCalculatingPath = false;

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

    private IEnumerator CheckPlacementValidityCoroutine(Vector3Int gridPosition, List<Vector2Int> cells, bool isPreview, System.Action<bool> callback)
    {
        // 임시로 노드들을 막음
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            aGrid.SetTemporaryNodeState(blockPos, false);
        }

        // 모든 스폰 포인트에서 동시에 경로 체크
        PathManager.Instance.CheckPreviewPath();

        bool anyValidPath = false;
        bool checkComplete = false;

        PathManager.Instance.UpdatePreviewPathWithDelay((isValid) => {
            anyValidPath = isValid;
            checkComplete = true;
        });

        // 코루틴으로 대기
        float timeout = Time.time + 0.5f;
        while (!checkComplete && Time.time < timeout)
        {
            yield return null;
        }

        // 노드 상태 복원
        aGrid.RestoreTemporaryNodes();

        if (!anyValidPath && isPreview)
        {
            Debug.Log("Block placement would block all possible paths");
        }

        callback(anyValidPath);
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
                
                if ((blockPos.x == spawnGridPos.x && blockPos.z == spawnGridPos.z) ||
                    (blockPos.x == targetGridPos.x && blockPos.z == targetGridPos.z))
                {
                    return false;
                }
            }
        }

        // 임시로 노드들을 막음
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            aGrid.SetTemporaryNodeState(blockPos, false);
        }

        // 경로 체크
        PathManager.Instance.CheckPreviewPath();
        bool isValid = PathManager.Instance.HasValidPath;

        // 노드 상태 복원
        aGrid.RestoreTemporaryNodes();

        return isValid && GridData.Instance.CanPlaceObjectAt(gridPosition, cells);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        int floor = 0;
        bool validity = false;
        
        if (database.IsTower(database.objectsData[selectedObjectIndex].ID))
        {
            Vector3Int positionBelow = new Vector3Int(gridPosition.x, 0, gridPosition.z);
            bool hasBlockBelow = BlockData.GetRepresentationIndex(positionBelow) != -1;
            bool hasObstacleBelow = false;
            bool canPlaceTower = false;

            // 장애물 체크
            Vector3 checkPosition = grid.CellToWorld(positionBelow) + new Vector3(0.5f, 0, 0.5f);
            Collider[] obstacles = Physics.OverlapSphere(checkPosition, 0.3f, aGrid.unwalkableMask);
            foreach (var obstacle in obstacles)
            {
                hasObstacleBelow = true;
                if (obstacle.CompareTag("TowerPlaceable"))
                {
                    canPlaceTower = true;
                    break;
                }
            }

            if (hasBlockBelow || (hasObstacleBelow && canPlaceTower))
            {
                floor = 1;
                validity = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                    floor);
            }
            else
            {
                validity = false; // 타워를 설치할 수 없는 위치
            }
        }
        else
        {
            validity = GridData.Instance.CanPlaceObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation)) &&
                CheckPlacementValidity(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                    true);
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

            Vector3 checkPosition = grid.CellToWorld(positionBelow) + new Vector3(0.5f, 0, 0.5f);
            Collider[] obstacles = Physics.OverlapSphere(checkPosition, 0.3f, aGrid.unwalkableMask);
            foreach (var obstacle in obstacles)
            {
                hasObstacleBelow = true;
                if (obstacle.CompareTag("TowerPlaceable"))
                {
                    canPlaceTower = true;
                    break;
                }
            }

            if ((hasBlockBelow || (hasObstacleBelow && canPlaceTower)))
            {
                floor = 1;
                canPlace = TowerData.CanPlaceObjectAt(gridPosition, 
                    database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                    floor);
            }
        }
        else
        {
            List<Vector2Int> cells = database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation);
            canPlace = CheckPlacementValidity(gridPosition, cells, false);
        }

        if (canPlace)
        {
            GridData selectedData = database.IsBlock(currentID) ? BlockData : TowerData;
            Vector3 position = grid.CellToWorld(gridPosition);
            position += new Vector3(0.5f, floor, 0.5f);

            // 블록 설치 전에 해당 위치의 나무 체크
            if (database.IsBlock(currentID))
            {
                foreach (var cell in database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation))
                {
                    Vector3 checkPosition = position + new Vector3(cell.x, 0, cell.y);
                    Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.5f);
                    foreach (var collider in colliders)
                    {
                        TreeDisapearEffect treeEffect = collider.GetComponent<TreeDisapearEffect>();
                        if (treeEffect != null)
                        {
                            ParticleSystem particle = UnityEngine.Object.Instantiate(treeEffect.effect, treeEffect.transform.position, Quaternion.identity);
                            UnityEngine.Object.Destroy(particle.gameObject, particle.main.duration);
                            UnityEngine.Object.Destroy(treeEffect.gameObject);
                        }
                    }
                }
            }

            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, 
                position, Quaternion.Euler(0, 90 * currentRotation, 0));

            if (database.IsBlock(currentID))
            {
                GameObject placedObject = ObjectPlacer.Instance.placedGameObjects[index];
                if (placedObject != null)
                {
                    placedObject.layer = LayerMask.NameToLayer("Unwalkable");
                }
            }

            selectedData.AddObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                currentID, index, floor);

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
                }
            }

            if (database.IsTower(currentID))
            {
                for (int i = 0; i < database.objectsData.Count; i++)
                {
                    if (database.objectsData[i].ID == currentID)
                    {
                        GameManager.Instance.CurrentMoney -= database.objectsData[i].Price;
                        break;
                    }
                }
            }
        }
    }

    public bool IsValidPlacement(Vector3Int gridPosition)
    {
        if (database.IsTower(database.objectsData[selectedObjectIndex].ID))
        {
            return CheckTowerPlacement(gridPosition);
        }
        else
        {
            return CheckBlockPlacement(gridPosition);
        }
    }

    private bool CheckTowerPlacement(Vector3Int gridPosition)
    {
        Vector3Int positionBelow = new Vector3Int(gridPosition.x, 0, gridPosition.z);
        bool hasBlockBelow = BlockData.GetRepresentationIndex(positionBelow) != -1;
        bool hasObstacleBelow = false;
        bool canPlaceTower = false;

        Vector3 checkPosition = grid.CellToWorld(positionBelow) + new Vector3(0.5f, 0, 0.5f);
        Collider[] obstacles = Physics.OverlapSphere(checkPosition, 0.3f, aGrid.unwalkableMask);
        foreach (var obstacle in obstacles)
        {
            hasObstacleBelow = true;
            if (obstacle.CompareTag("TowerPlaceable"))
            {
                canPlaceTower = true;
                break;
            }
        }

        return (hasBlockBelow || (hasObstacleBelow && canPlaceTower)) &&
               TowerData.CanPlaceObjectAt(gridPosition, 
                   database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation), 
                   1);
    }

    private bool CheckBlockPlacement(Vector3Int gridPosition)
    {
        List<Vector2Int> cells = database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation);
        return GridData.Instance.CanPlaceObjectAt(gridPosition, cells) &&
               CheckPlacementValidity(gridPosition, cells, true);
    }

    public void SetTemporaryNodes(Vector3Int gridPosition, bool walkable)
    {
        List<Vector2Int> cells = database.objectsData[selectedObjectIndex].GetRotatedCells(currentRotation);
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            aGrid.SetTemporaryNodeState(blockPos, walkable);
        }
    }

    public void RestoreTemporaryNodes()
    {
        aGrid.RestoreTemporaryNodes();
    }
}
