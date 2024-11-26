using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Enemy : MonoBehaviour
{
    private List<Node> path; // A* 알고리즘으로 계산된 경로
    private int pathIndex; // 현재 경로에서 이동 중인 노드의 인덱스
    private Transform targetBase; // 적이 목표로 삼는 기지의 Transform
    private Pathfinding_AstarMove pathfinding; // A* 알고리즘 경로 탐색을 담당하는 Pathfinding 스크립트 참조
    public float moveSpeed = 3.5f; // 적의 이동 속도

    private LineRenderer lineRenderer; // 경로를 시각화할 LineRenderer

    private void Awake()
    {
        // LineRenderer 초기화
        lineRenderer = GetComponent<LineRenderer>(); // 현재 GameObject에서 LineRenderer 스크립트를 가져옴

        // LineRenderer의 선 두께 설정
        lineRenderer.startWidth = 0.1f; // 선의 시작 부분 두께 설정
        lineRenderer.endWidth = 0.1f;   // 선의 끝 부분 두께 설정

        // LineRenderer의 머티리얼 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // LineRenderer에 사용할 기본 머티리얼 할당

        // LineRenderer의 색상 설정
        lineRenderer.startColor = Color.green; // 선의 시작 부분 색상을 녹색으로 설정
        lineRenderer.endColor = Color.red;     // 선의 끝 부분 색상을 빨간색으로 설정
    }

    // Enemy의 초기화 메서드. 목표 기지와 Pathfinding 스크립트를 설정합니다.
    public void Initialize(Transform baseTransform, Pathfinding_AstarMove pathfindingComponent)
    {
        targetBase = baseTransform; // 목표 기지 설정
        pathfinding = pathfindingComponent; // Pathfinding 스크립트 설정

        // Pathfinding과 TargetBase가 정상적으로 초기화되었는지 확인
        if (pathfinding != null && targetBase != null)
        {
            // A* 알고리즘을 사용하여 경로를 계산
            path = pathfinding.FindPath(transform.position, targetBase.position);
            pathIndex = 0; // 경로의 시작 인덱스를 0으로 설정

            // 경로를 LineRenderer로 시각화
            DrawPath();
        }
        else
        {
            // 초기화 실패 시 오류 메시지 출력
            Debug.LogError("Pathfinding 스크립트 또는 목표 기지가 Enemy에서 초기화되지 않았습니다.");
        }
    }

    // 매 프레임마다 호출되어 적을 이동시키는 메서드
    private void Update()
    {
        // 경로가 존재하고, 아직 경로를 따라가는 중인지 확인
        if (path != null && pathIndex < path.Count)
        {
            // 현재 이동해야 할 노드의 위치
            Vector3 targetPos = path[pathIndex].worldPosition;

            // MoveTowards를 사용하여 현재 위치에서 목표 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 목표 위치에 도달하면 다음 노드로 이동
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                pathIndex++; // 다음 노드로 이동
                //UpdatePathVisualization(); // 이동 경로 업데이트
            }
        }
        else if (pathIndex >= path.Count) // 경로의 마지막 노드에 도달한 경우
        {
            // 디버그 메시지 출력
            Debug.Log("목표 지점에 도착, Enemy 오브젝트 삭제");
            Destroy(gameObject); // Enemy 오브젝트 삭제
        }
    }

    // 경로를 LineRenderer로 시각화
    private void DrawPath()
    {
        if (path != null && path.Count > 0)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, path[i].worldPosition);
            }
        }
    }

    // 이동 중인 경로를 실시간으로 업데이트
    private void UpdatePathVisualization()
    {
        if (lineRenderer.positionCount > pathIndex)
        {
            // 이미 지나온 경로를 투명하게 처리
            lineRenderer.startColor = Color.clear;
            lineRenderer.endColor = Color.red;
        }
    }
}
