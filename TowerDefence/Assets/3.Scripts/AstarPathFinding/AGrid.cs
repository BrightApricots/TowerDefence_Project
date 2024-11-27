using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    private ANode[,] nodeArray;

    [SerializeField]
    private List<Vector2Int> obstaclePositions = new List<Vector2Int>();

    int gridSizeX, gridSizeY;

    [SerializeField]
    private ObjectsDatabaseSO database;
    [SerializeField]
    private Grid gridComponent;

    private GridData BlockData;

    // 노드의 임시 상태를 저장할 Dictionary 추가
    private Dictionary<Vector3Int, bool> temporaryNodeStates = new Dictionary<Vector3Int, bool>();

    void Awake()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y);
        BlockData = new GridData();
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        nodeArray = new ANode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        worldBottomLeft.y = transform.position.y;

        if (ObjectPlacer.Instance == null)
        {
            GameObject placerObj = new GameObject("ObjectPlacer");
            placerObj.AddComponent<ObjectPlacer>();
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x + 0.5f) + Vector3.forward * (y + 0.5f);
                worldPoint.y = transform.position.y;

                Collider[] hitColliders = Physics.OverlapSphere(worldPoint, .3f, unwalkableMask);
                bool isBlocked = false;

                if (hitColliders.Length > 0 && hitColliders[0] != null)
                {
                    GameObject obstacleObject = hitColliders[0].gameObject;
                    
                    // 블록 템플릿과 프리뷰는 무시
                    if (!obstacleObject.name.Contains("blockTemplate") && 
                        !obstacleObject.name.Contains("Template") &&
                        !obstacleObject.name.Contains("Preview") &&
                        !obstacleObject.CompareTag("Preview"))
                    {
                        isBlocked = true;
                        Debug.Log($"Found obstacle: {obstacleObject.name}");
                        
                        if (!ObjectPlacer.Instance.placedGameObjects.Contains(obstacleObject))
                        {
                            ObjectPlacer.Instance.placedGameObjects.Add(obstacleObject);
                            Debug.Log($"Added to placedGameObjects. Count: {ObjectPlacer.Instance.placedGameObjects.Count}");
                        }

                        Vector3Int gridPosition = new Vector3Int(x - gridSizeX/2, 0, y - gridSizeY/2);
                        List<Vector2Int> occupiedCells = new List<Vector2Int> { new Vector2Int(0, 0) };
                        
                        int index = ObjectPlacer.Instance.placedGameObjects.IndexOf(obstacleObject);
                        GridData.Instance.AddObjectAt(gridPosition, occupiedCells, 0, index);
                        BlockData.AddObjectAt(gridPosition, occupiedCells, 0, index);
                    }
                    else
                    {
                        Debug.Log($"Ignoring template/preview object: {obstacleObject.name}");
                    }
                }

                bool walkable = !isBlocked;
                nodeArray[x, y] = new ANode(walkable, worldPoint, x, y);
            }
        }
    }

    public List<ANode> GetNeighbours(ANode node)
    {
        List<ANode> neighbours = new List<ANode>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),  // 상
            new Vector2Int(0, -1), // 하
            new Vector2Int(-1, 0), // 좌
            new Vector2Int(1, 0)   // 우
        };

        foreach (Vector2Int dir in directions)
        {
            int checkX = node.gridX + dir.x;
            int checkY = node.gridY + dir.y;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                neighbours.Add(nodeArray[checkX, checkY]);
            }
        }

        return neighbours;
    }

    public ANode ANodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return nodeArray[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (nodeArray != null && displayGridGizmos)
        {
            foreach (ANode n in nodeArray)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.1f);
            }
        }
    }

    public void UpdateGrid()
    {
        CreateGrid();
    }

    public void UpdateNode(Vector3Int position, bool isBlocked)
    {
        int gridX = position.x + gridSizeX / 2;
        int gridY = position.z + gridSizeY / 2;

        if (gridX >= 0 && gridX < gridSizeX && gridY >= 0 && gridY < gridSizeY)
        {
            nodeArray[gridX, gridY].walkable = !isBlocked;
            Debug.Log($"Node updated at grid coordinates ({gridX}, {gridY}), walkable: {!isBlocked}");
        }
        else
        {
            Debug.LogWarning($"Attempted to update node outside grid bounds at ({gridX}, {gridY})");
        }
    }

    // 노드의 임시 상태 설정
    public void SetTemporaryNodeState(Vector3Int position, bool walkable)
    {
        int gridX = position.x + gridSizeX / 2;
        int gridY = position.z + gridSizeY / 2;

        if (gridX >= 0 && gridX < gridSizeX && gridY >= 0 && gridY < gridSizeY)
        {
            temporaryNodeStates[position] = nodeArray[gridX, gridY].walkable;
            nodeArray[gridX, gridY].walkable = walkable;
        }
    }

    // 임시 상태 복원
    public void RestoreTemporaryNodes()
    {
        foreach (var kvp in temporaryNodeStates)
        {
            int gridX = kvp.Key.x + gridSizeX / 2;
            int gridY = kvp.Key.z + gridSizeY / 2;

            if (gridX >= 0 && gridX < gridSizeX && gridY >= 0 && gridY < gridSizeY)
            {
                nodeArray[gridX, gridY].walkable = kvp.Value;
            }
        }
        temporaryNodeStates.Clear();
    }

    private bool CheckPathValidity(Vector3Int gridPosition, List<Vector2Int> cells)
    {
        // 임시로 노드들을 막음
        Dictionary<ANode, bool> originalStates = new Dictionary<ANode, bool>();
        foreach (var cell in cells)
        {
            Vector3Int blockPos = new Vector3Int(
                gridPosition.x + cell.x,
                gridPosition.y,
                gridPosition.z + cell.y
            );
            ANode node = ANodeFromWorldPoint(new Vector3(blockPos.x, 0, blockPos.z));
            if (node != null && !originalStates.ContainsKey(node))
            {
                originalStates[node] = node.walkable;
                node.walkable = false;
            }
        }

        // 프리뷰 경로 업데이트 전에 모든 스폰 포인트에서 경로 체크
        bool anyValidPath = false;
        foreach (var spawnPoint in PathManager.Instance.GetSpawnPoints())
        {
            PathManager.Instance.CheckPreviewPath();
            if (PathManager.Instance.HasValidPath)
            {
                anyValidPath = true;
                break;
            }
        }

        // 노드 상태 복원
        foreach (var pair in originalStates)
        {
            pair.Key.walkable = pair.Value;
        }

        return anyValidPath;
    }
}