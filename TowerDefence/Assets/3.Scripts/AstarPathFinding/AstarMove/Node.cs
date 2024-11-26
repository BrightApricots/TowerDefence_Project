using UnityEngine;

public class Node
{
    public bool walkable; // 노드가 이동 가능한지 여부를 나타냄
    public Vector3 worldPosition; // 노드의 월드 좌표
    public int gridX, gridY; // 노드의 격자 좌표 (그리드 상 위치)

    public int gCost, hCost; // A* 알고리즘에서 사용하는 비용
    public Node parent; // 경로를 추적하기 위한 부모 노드

    // 노드 생성자: 이동 가능 여부, 월드 좌표, 격자 좌표를 설정
    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable; // 이동 가능 여부 설정
        this.worldPosition = worldPosition; // 월드 좌표 설정
        this.gridX = gridX; // 격자 X 좌표 설정
        this.gridY = gridY; // 격자 Y 좌표 설정
    }

    // 총 비용 계산: gCost(현재까지의 비용) + hCost(예상 비용)
    public int fCost => gCost + hCost;
}
