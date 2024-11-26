// 요약 : 노드 클래스 

using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    public LayerMask unwalkableMask; // 이동 불가능한 영역을 나타내는 레이어
    public Vector2 gridWorldSize;    // 그리드의 월드 크기
    public float nodeRadius;         // 각 노드의 반지름

    private Node[,] grid; // 그리드의 노드 배열
    private float nodeDiameter; // 노드의 지름 (반지름 * 2)
    private int gridSizeX, gridSizeY; // 그리드의 X, Y 크기 (노드 개수)

    public List<Node> path; // 계산된 경로를 저장

    public bool isLook = false; // 노드 표시 여부 

    // 시작 시 그리드를 초기화
    void Start()
    {
        nodeDiameter = nodeRadius * 2; // 노드의 지름 계산
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); // X 방향 노드 개수 계산
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); // Y 방향 노드 개수 계산
        CreateGrid(); // 그리드 생성
    }

    // 그리드 생성 메서드
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; // 노드 배열 초기화
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) // X축 순회
        {
            for (int y = 0; y < gridSizeY; y++) // Y축 순회
            {
                // 각 노드의 월드 좌표 계산
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                // 노드가 이동 가능한지 확인
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y); // 노드 생성
            }
        }
    }

    // 월드 좌표에서 해당 위치의 노드를 반환
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // 월드 좌표를 그리드 좌표로 변환
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y]; // 해당 노드 반환
    }

    // 특정 노드의 이웃 노드 리스트 반환
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>(); // 이웃 노드 리스트 초기화

        for (int x = -1; x <= 1; x++) // 주변 노드를 검사 (-1, 0, 1)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) // 자기 자신은 제외
                    continue;

                int checkX = node.gridX + x; // 검사할 X 위치
                int checkY = node.gridY + y; // 검사할 Y 위치

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) // 그리드 범위 확인
                {
                    neighbors.Add(grid[checkX, checkY]); // 이웃 노드 추가
                }
            }
        }

        return neighbors; // 이웃 노드 리스트 반환
    }

    // *런타임 중 새로운 오브젝트로 인해 이동 불가능한 영역을 업데이트
    public void UpdateGrid(Vector3 objectPosition)
    {
        Node node = NodeFromWorldPoint(objectPosition); // 1. 전달 받은 새로 생성된 오브젝트의 위치를 기반으로 노드 탐색
        if (node != null)
        {
            // 해당 노드를 이동 불가능한 상태로 변경
            bool walkable = !Physics.CheckSphere(node.worldPosition, nodeRadius, unwalkableMask); // 2. 노드의 이동 가능 여부 판단
            node.walkable = walkable; //3. 노드를 이동 불가능 영역으로 확정
        }
    }

    // 그리드를 시각적으로 표시
    void OnDrawGizmos()
    {
        if (isLook == false) // 화면에 노드 표시 여부
        {// 그리드 전체 영역을 박스로 표시
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid != null) // 그리드가 존재할 경우 
            {
                foreach (Node n in grid)
                {
                    // 이동 가능 여부에 따라 색상 변경
                    Gizmos.color = n.walkable ? Color.white : Color.red;
                    if (path != null && path.Contains(n)) // 경로에 포함된 노드면 검정색
                        Gizmos.color = Color.black;
                    // 노드를 박스로 표시
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }
}
