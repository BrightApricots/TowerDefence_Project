using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Enemy : MonoBehaviour
{
    private List<Node> path; // A* 경로 탐색을 위한 리스트
    private int pathIndex; // 현재 경로에서 이동할 인덱스
    private Transform targetBase; // 목표 지점의 Transform
    private Pathfinding_AstarMove pathfinding; // A* 경로 탐색을 위한 Pathfinding 컴포넌트
    public float moveSpeed = 3.5f; // 이동 속도
    public int Hp; // 체력
    public int Speed; // 속도
    public int Damage; // 피해량
    private UnityEngine.AI.NavMeshAgent agent; // NavMeshAgent

    private LineRenderer lineRenderer; // 경로 시각화를 위한 LineRenderer

    private void Awake()
    {
        // LineRenderer 초기화
        lineRenderer = GetComponent<LineRenderer>(); // 현재 GameObject에서 LineRenderer 컴포넌트를 가져옴

        // LineRenderer의 시작과 끝 두께 설정
        lineRenderer.startWidth = 0.1f; // 시작 두께
        lineRenderer.endWidth = 0.1f;   // 끝 두께

        // LineRenderer의 재질 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // LineRenderer의 기본 재질 설정

        // LineRenderer의 색상 설정
        lineRenderer.startColor = Color.green; // 시작 색상
        lineRenderer.endColor = Color.red;     // 끝 색상
    }

    // Enemy 초기화 메서드. 목표 지점과 Pathfinding 컴포넌트를 설정합니다.
    public void Initialize(Transform baseTransform, Pathfinding_AstarMove pathfindingComponent)
    {
        targetBase = baseTransform; // 목표 지점 설정
        pathfinding = pathfindingComponent; // Pathfinding 컴포넌트 설정

        // Pathfinding이 TargetBase를 올바르게 초기화했는지 확인
        if (pathfinding != null && targetBase != null)
        {
            // A* 경로 탐색을 통해 경로 찾기
            path = pathfinding.FindPath(transform.position, targetBase.position);
            pathIndex = 0; // 현재 인덱스 초기화

            // LineRenderer로 경로 그리기
            DrawPath();
        }
        else
        {
            // 초기화 중 오류 발생
            Debug.LogError("Pathfinding 컴포넌트가 없거나 목표 지점이 설정되지 않았습니다.");
        }
    }

    // 매 프레임 업데이트
    private void Update()
    {
        // 경로가 존재하고 인덱스가 유효한 경우
        if (path != null && pathIndex < path.Count)
        {
            // 현재 목표 위치
            Vector3 targetPos = path[pathIndex].worldPosition;

            // MoveTowards를 사용하여 목표 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 목표 위치에 도달했는지 확인
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                pathIndex++; // 다음 인덱스로 이동
                //UpdatePathVisualization(); // 경로 시각화 업데이트
            }
        }
        else if (pathIndex >= path.Count) // 모든 경로를 다 이동한 경우
        {
            // 적 제거
            Debug.Log("목표에 도달했습니다, Enemy 오브젝트 제거");
            Destroy(gameObject); // Enemy 오브젝트 제거
        }
    }

    // LineRenderer로 경로 그리기
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

    // 이동 경로 시각화 업데이트
    private void UpdatePathVisualization()
    {
        if (lineRenderer.positionCount > pathIndex)
        {
            // 경로 시각화 색상 변경
            lineRenderer.startColor = Color.clear;
            lineRenderer.endColor = Color.red;
        }
    }
}