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
        PlacementData placementData = null;

        // 블록 데이터 확인
        placementData = BlockData.GetPlacementData(gridPosition);
        if (placementData != null)
        {
            selectedData = BlockData;
            index = placementData.PlacedObjectIndex;
            id = placementData.ID;
        }
        else
        {
            // 타워 데이터 확인
            placementData = TowerData.GetPlacementData(gridPosition);
            if (placementData != null)
            {
                selectedData = TowerData;
                index = placementData.PlacedObjectIndex;
                id = placementData.ID;
            }
        }

        if (selectedData != null && placementData != null)
        {
            // 오브젝트 제거 전에 모든 점유 셀의 노드 상태를 업데이트
            foreach (var pos in placementData.occupiedPositions)
            {
                // 노드 상태를 walkable로 변경
                aGrid.UpdateNode(pos, false);
                
                // GridData에서도 해당 위치 제거
                selectedData.RemoveObjectAt(pos);
            }

            // 오브젝트 제거
            objectPlacer.RemoveObjectAt(index);

            // PlacementSystem에 경로 업데이트 요청
            PlacementSystem.Instance.RequestPathUpdate();
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