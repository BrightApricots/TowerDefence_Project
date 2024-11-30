using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] // 변수 인스펙터에 노출
    private int hp = 100; // 몬스터 체력
    [SerializeField]
    private float speed = 2f; // 몬스터 이동 속도
    [SerializeField]
    private int damage = 1; // 몬스터 데미지
    [SerializeField]
    private int gold = 1; // 몬스터 처치 시 골드

    private Vector3[] path; // 이동 경로 배열
    private int currentWaypointIndex; // 현재 목표 웨이포인트 인덱스
    private bool isMoving = false; // 이동 활성화 여부
    private Vector3 currentTargetPosition; // 현재 목표 위치
    private Vector3 moveDirection; // 이동 방향
    private Transform spawnPoint; // 스폰 지점

    // 몬스터가 제거될 때 호출되는 이벤트
    public event System.Action OnDestroyed;

    // 초기화: 스폰 지점 설정 및 경로 가져오기
    public void Initialize(Transform spawn)
    {
        spawnPoint = spawn;
        transform.position = PathManager.Instance.GetSpawnPosition(spawn); // 스폰 지점 위치 설정
        Vector3[] newPath = PathManager.Instance.GetCurrentPath(spawn); // 이동 경로 가져오기

        if (newPath != null && newPath.Length > 0)
        {
            path = newPath;
            currentWaypointIndex = 0; // 첫 번째 웨이포인트부터 시작
            isMoving = true; // 이동 활성화

            if (PathManager.Instance != null)
            {
                PathManager.Instance.OnPathUpdated += OnPathUpdated; // 경로 업데이트 이벤트 구독
            }

            UpdateCurrentTarget(); // 초기 목표 위치 설정
        }
        else
        {
            Debug.LogWarning("유효한 경로를 찾을 수 없습니다. 몬스터 초기화 실패."); // 경고 메시지 출력
        }
    }

    // 경로 업데이트 이벤트 처리
    private void OnPathUpdated(Transform updatedSpawnPoint, Vector3[] newPath)
    {
        // 현재 스폰 지점의 경로가 업데이트된 경우 처리
        if (spawnPoint == updatedSpawnPoint && newPath != null && newPath.Length > 0)
        {
            moveDirection = (currentTargetPosition - transform.position).normalized; // 기존 이동 방향 유지

            path = newPath; // 새로운 경로 설정
            float bestScore = float.MinValue;
            int bestIndex = 0;

            // 최적의 웨이포인트 인덱스 계산
            for (int i = 0; i < path.Length; i++)
            {
                Vector3 toWaypoint = (path[i] - transform.position);
                float distance = toWaypoint.magnitude;
                float dotProduct = Vector3.Dot(moveDirection, toWaypoint.normalized);

                float score = dotProduct / (distance + 0.1f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            if (bestIndex > 0 && bestScore < -0.5f)
            {
                bestIndex--; // 이전 웨이포인트로 조정
            }

            currentWaypointIndex = bestIndex; // 최적의 인덱스 설정
            UpdateCurrentTarget(); // 목표 위치 업데이트
        }
    }

    // 현재 목표 위치 업데이트
    private void UpdateCurrentTarget()
    {
        // 다음 웨이포인트 또는 최종 목표 위치 설정
        if (path != null && currentWaypointIndex < path.Length)
        {
            currentTargetPosition = path[currentWaypointIndex]; // 현재 웨이포인트 위치
            currentTargetPosition.y = transform.position.y; // 높이 유지
        }
        else
        {
            currentTargetPosition = PathManager.Instance.GetTargetPosition(); // 최종 목표 위치
            currentTargetPosition.y = transform.position.y; // 높이 유지
        }
    }

    // 매 프레임마다 이동 처리
    void Update()
    {
        if (!isMoving) return; // 이동이 비활성화된 경우 종료

        Vector3 targetDirection = (currentTargetPosition - transform.position).normalized; // 목표 방향 계산
        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * 10f); // 부드러운 방향 전환

        transform.position += moveDirection * speed * Time.deltaTime; // 이동

        // 현재 목표에 도달했는지 확인
        if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
        {
            if (currentWaypointIndex < path.Length - 1)
            {
                currentWaypointIndex++; // 다음 웨이포인트로 이동
                UpdateCurrentTarget(); // 목표 위치 업데이트
            }
            else
            {
                Vector3 finalTarget = PathManager.Instance.GetTargetPosition();
                finalTarget.y = transform.position.y; // 높이 유지

                // 최종 목표에 도달했는지 확인
                if (Vector3.Distance(transform.position, finalTarget) < 0.1f)
                {
                    OnReachedTarget(); // 목표 도달 처리
                }
            }
        }
    }

    // 목표에 도달했을 때 호출
    private void OnReachedTarget()
    {
        // GameManager의 체력 감소 추가 처리
        GameManager.Instance.CurrentHp -= damage; // 체력 감소

        // 디버깅 메시지 출력
        Debug.Log($"{gameObject.name} / 목표 지점에 도달 / {damage}만큼 체력을 감소");

        // 이벤트 구독 해제 및 몬스터 삭제
        if (PathManager.Instance != null)
        {
            PathManager.Instance.OnPathUpdated -= OnPathUpdated;
        }
        Destroy(gameObject); // 몬스터 삭제
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 남은 HP : {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"획득 골드 : {gold}");
        GameManager.Instance.CurrentMoney += gold;
        Destroy(gameObject);
    }

    // 몬스터가 제거될 때
    void OnDestroy()
    {
        // PathManager의 이벤트 구독 해제
        if (PathManager.Instance != null)
        {
            PathManager.Instance.OnPathUpdated -= OnPathUpdated;
        }

        // 몬스터 제거 이벤트 호출
        OnDestroyed?.Invoke();
    }
}
