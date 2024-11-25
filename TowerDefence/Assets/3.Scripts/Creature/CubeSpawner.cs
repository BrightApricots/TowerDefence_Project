using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // 생성할 큐브 프리팹
    public Camera mainCamera;     // 클릭 감지를 위한 카메라

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 클릭 감지
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // 화면 좌표 -> 월드 레이
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // 레이가 충돌한 위치
            {
                // 충돌 지점에 큐브 생성
                GameObject newCube = Instantiate(cubePrefab, hit.point, Quaternion.identity);

                // 생성된 큐브의 y 값을 0.6으로 고정
                Vector3 fixedPosition = newCube.transform.position;
                fixedPosition.y = 0.6f; // y값 설정
                newCube.transform.position = fixedPosition;
            }
        }
    }
}

/*
 * using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;  // NavMeshAgent 컴포넌트
    private Transform targetBase; // 플레이어 기지의 Transform

    [Header("Enemy Settings")]
    public float moveSpeed = 3.5f; // 적 이동 속도

    public void Initialize(Transform baseTransform)
    {
        // 플레이어 기지의 Transform을 설정
        targetBase = baseTransform;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 이동 속도 설정
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (targetBase != null)
        {
            // 기지로 이동
            agent.SetDestination(targetBase.position);
        }
    }

    // Gizmos를 사용하여 경로를 시각적으로 표시
    private void OnDrawGizmos()
    {
        if (agent == null || agent.path == null)
            return;

        // 경로를 녹색 선으로 표시
        Gizmos.color = Color.green;
        Vector3 previousCorner = transform.position; // 시작 지점

        foreach (var corner in agent.path.corners)
        {
            Gizmos.DrawLine(previousCorner, corner); // 이전 지점에서 다음 지점까지 선 그리기
            previousCorner = corner;
        }
    }
}  여기서 그리드 중심으로 이동하는 경로탐색을 구현해줘
 */