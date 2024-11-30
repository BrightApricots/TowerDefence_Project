using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridData
{
    private static GridData instance;
    public static GridData Instance
    {
        get
        {
            if (instance == null)
                instance = new GridData();
            return instance;
        }
    }

    private Dictionary<Vector3Int, PlacementData> placedObjects = new();
    private ObjectPlacer placer;

    public bool HasObjectAt(Vector3Int position)
    {
        return placedObjects.ContainsKey(position);
    }

    public PlacementData GetObjectAt(Vector3Int position)
    {
        if (placedObjects.ContainsKey(position))
            return placedObjects[position];
        return null;
    }

    public void AddObjectAt(Vector3Int gridPosition, List<Vector2Int> occupiedCells, int ID, int placedObjectIndex, int floor = 0)
    {
        gridPosition.y = floor;
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, occupiedCells);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    public void UpdateMapObject(Vector2 placedPosition)
    {
        Vector3Int newPos = new Vector3Int((int)placedPosition.x, 0, (int)placedPosition.y);

        List<Vector2Int> occupiedCells = new List<Vector2Int> { new Vector2Int(0, 0) };
        List<Vector3Int> positionToOccupy = CalculatePositions(newPos, occupiedCells);

        PlacementData data = new PlacementData(positionToOccupy, 0, 0);


        placedObjects.Add(newPos, data);

        GameObject go = new GameObject();
        go.transform.position = newPos;
        go.transform.rotation = Quaternion.identity;
        ObjectPlacer.Instance.placedGameObjects.Add(go);

        foreach(var pos in placedObjects.Keys)
        {
            Debug.Log($"placedObjects : {pos.x}, {pos.y}, {pos.z}");
        }

        //foreach (var pos in positionToOccupy)
        //{
        //    if (placedObjects.ContainsKey(pos))
        //        throw new Exception($"Dictionary already contains this cell position {pos}");
        //    placedObjects[pos] = data;
        //}

        //// 이미 해당 위치에 오브젝트가 있는지 확인
        //if (!placedObjects.ContainsKey(newPos))
        //{
        //    AddObjectAt(newPos, occupiedCells, 1, 0, 0); // ID와 placedObjectIndex는 임의의 값 사용
        //}
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, List<Vector2Int> occupiedCells)
    {
        List<Vector3Int> returnVal = new();
        foreach (var cell in occupiedCells)
        {
            returnVal.Add(new Vector3Int(gridPosition.x + cell.x,gridPosition.y, gridPosition.z + cell.y));
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, List<Vector2Int> occupiedCells, int floor = 0)
    {
        gridPosition.y = floor;
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, occupiedCells);
        
        foreach (var pos in positionToOccupy)
        {
            if (IsPositionOccupied(pos))
                return false;
        }
        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    public PlacementData GetPlacementData(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition))
            return placedObjects[gridPosition];
        return null;
    }

    public bool IsPositionOccupied(Vector3Int position)
    {
        return placedObjects.ContainsKey(position);
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}