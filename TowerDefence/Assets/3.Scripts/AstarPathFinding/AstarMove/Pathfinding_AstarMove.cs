using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_AstarMove : MonoBehaviour
{
    private CustomGrid grid; // CustomGrid 스크립트 참조

    void Awake()
    {
        // Grid 스크립트를 동적으로 찾기
        grid = FindObjectOfType<CustomGrid>();
        if (grid == null)
        {
            Debug.LogError("씬에 CustomGrid 스크립트가 없습니다. 적절한 GameObject에 CustomGrid 스크립트를 추가하세요.");
        }
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // grid가 null이면 경로 탐색 중단
        if (grid == null)
        {
            Debug.LogError("CustomGrid가 초기화되지 않았습니다. CustomGrid 스크립트를 확인하세요.");
            return null;
        }

        // 시작 노드와 목표 노드를 찾음
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // 열린 노드 집합과 닫힌 노드 집합 생성
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // 현재 노드와 비교하여 비용(fCost)이 더 낮은 노드를 선택
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            // 현재 노드를 열린 집합에서 제거하고 닫힌 집합에 추가
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // 목표 노드에 도달한 경우 경로를 반환
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // 현재 노드의 이웃 노드 검사
            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                // 이동 불가능한 노드이거나 이미 닫힌 집합에 있는 경우 건너뜀
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                // 현재 노드를 거쳐 이웃 노드로 가는 비용 계산
                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // 비용 갱신
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode; // 부모 노드 설정

                    // 열린 집합에 추가 (중복 방지)
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null; // 경로를 찾지 못한 경우 null 반환
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        // 시작 노드부터 목표 노드까지의 경로를 추적
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode); // 현재 노드를 경로에 추가
            currentNode = currentNode.parent; // 부모 노드로 이동
        }
        path.Reverse(); // 경로를 역순으로 정렬 (시작 노드부터 목표 노드까지)

        grid.path = path; // Grid에 경로 저장 (디버깅용)
        return path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        // 두 노드 간의 거리 계산
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // 대각선 이동 비용(14)과 직선 이동 비용(10) 계산
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
