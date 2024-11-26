using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData BlockData;
    GridData TowerData;
    ObjectPlacer objectPlacer;
    AGrid aGrid;
    InputManager inputManager;

    public RemovingState(Grid grid, PreviewSystem previewSystem, GridData blockData, GridData towerData, ObjectPlacer objectPlacer, AGrid aGrid, InputManager inputManager)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.BlockData = blockData;
        this.TowerData = towerData;
        this.objectPlacer = objectPlacer;
        this.aGrid = aGrid;
        this.inputManager = inputManager;
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        GridData selectedData = null;
        int index = -1;
        int id = -1;

        // 블록 데이터 확인
        PlacementData blockData = BlockData.GetPlacementData(gridPosition);
        if (blockData != null)
        {
            selectedData = BlockData;
            index = blockData.PlacedObjectIndex;
            id = blockData.ID;
        }
        else
        {
            // 타워 데이터 확인
            PlacementData towerData = TowerData.GetPlacementData(gridPosition);
            if (towerData != null)
            {
                selectedData = TowerData;
                index = towerData.PlacedObjectIndex;
                id = towerData.ID;
            }
        }

        if (selectedData != null)
        {
            // 오브젝트 제거
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(index);

            // AGrid 업데이트
            foreach (var pos in blockData.occupiedPositions)
            {
                aGrid.UpdateNode(pos, false);
            }

            // 경로 업데이트
            if (PathManager.Instance != null)
            {
                PathManager.Instance.UpdateAllPaths();
            }
        }
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return TowerData.GetRepresentationIndex(gridPosition) != -1 ||  BlockData.GetRepresentationIndex(gridPosition) != -1;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }

    public void OnAction(object gridpo)
    {
        throw new NotImplementedException();
    }
}