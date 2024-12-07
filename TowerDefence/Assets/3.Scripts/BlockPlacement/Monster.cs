using TMPro;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private int hp = 100;
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private int gold = 1;
    [SerializeField]
    private float rotationSpeed = 5f; // 회전 속도

    private Vector3[] path;
    private int currentWaypointIndex;
    private bool isMoving = false;
    private Vector3 currentTargetPosition;
    private Vector3 moveDirection;
    private Transform spawnPoint;

    public event System.Action OnDestroyed;

    public void Initialize(Transform spawn)
    {
        spawnPoint = spawn;
        transform.position = PathManager.Instance.GetSpawnPosition(spawnPoint);
        Vector3[] newPath = PathManager.Instance.GetActualPath(spawnPoint);
        
        if (newPath != null && newPath.Length > 0)
        {
            path = newPath;
            currentWaypointIndex = 0;
            isMoving = true;
            
            if (PathManager.Instance != null)
            {
                PathManager.Instance.OnActualPathUpdated += OnActualPathUpdated;
            }
            
            UpdateCurrentTarget();
        }
        else
        {
            Debug.LogWarning("No valid path found for monster initialization");
        }
    }

    private void OnActualPathUpdated(Transform updatedSpawnPoint, Vector3[] newPath)
    {
        if (spawnPoint == updatedSpawnPoint && newPath != null && newPath.Length > 0)
        {
            moveDirection = (currentTargetPosition - transform.position).normalized;
            
            path = newPath;
            float bestScore = float.MinValue;
            int bestIndex = 0;

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
                bestIndex--;
            }

            currentWaypointIndex = bestIndex;
            UpdateCurrentTarget();
        }
    }

    private void UpdateCurrentTarget() // 현재 목표 위치
    {
        if (path != null && currentWaypointIndex < path.Length)
        {
            currentTargetPosition = path[currentWaypointIndex];
            currentTargetPosition.y = transform.position.y;
        }
        else
        {
            currentTargetPosition = PathManager.Instance.GetTargetPosition();
            currentTargetPosition.y = transform.position.y;
        }
    }

    void Update()
    {
        if (!isMoving) return;

        Vector3 targetDirection = (currentTargetPosition - transform.position).normalized; // 현재 목표 위치 - 몬스터의 현재 위치
        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * 10);

        // 이동
        transform.position += moveDirection * speed * Time.deltaTime;

        // 회전 설정
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
        {
            if (currentWaypointIndex < path.Length - 1)
            {
                currentWaypointIndex++;
                UpdateCurrentTarget();
            }
            else
            {
                Vector3 finalTarget = PathManager.Instance.GetTargetPosition();
                finalTarget.y = transform.position.y;

                if (Vector3.Distance(transform.position, finalTarget) < 0.1f)
                {
                    OnReachedTarget();
                }
            }
        }
    }

    private void OnReachedTarget()
    {
        GameManager.Instance.CurrentHp -= damage;
        Debug.Log($"{gameObject.name} / 목표 지점에 도달 / {damage}만큼 체력을 감소");

        if (PathManager.Instance != null)
        {
            PathManager.Instance.OnActualPathUpdated -= OnActualPathUpdated;
        }
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        try 
        {
            GameObject damageFontPrefab = Resources.Load<GameObject>("Effects/DamageFont");
            if (damageFontPrefab == null)
            {
                Debug.LogError("DamageFont 프리프를 찾을 수 없습니다!");
                return;
            }

            GameObject damageCanvas = GameObject.Find("DamageCanvas");
            if (damageCanvas == null)
            {
                Debug.LogError("DamageCanvas를 찾을 수 없습니다!");
                return;
            }

            DamageFontEffect damageEffect = ObjectManager.Instance.Spawn<DamageFontEffect>(
                damageFontPrefab, 
                Vector3.zero,
                Quaternion.identity
            );

            if (damageEffect == null)
            {
                Debug.LogError("DamageEffect를 생성할 수 없습니다!");
                return;
            }

            damageEffect.transform.SetParent(damageCanvas.transform, false);
            damageEffect.SetDamageText(damage.ToString(), transform.position);

            hp -= damage;
            Debug.Log($"{gameObject.name} 남은 HP : {hp}");

            if (hp <= 0)
            {
                Die();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데미지 효과 생성 중 에러 발생: {e.Message}");
        }
    }

    private void Die()
    {
        Debug.Log($"획득 골드 : {gold}");
        GameManager.Instance.CurrentMoney += gold;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (PathManager.Instance != null)
        {
            PathManager.Instance.OnActualPathUpdated -= OnActualPathUpdated;
        }

        OnDestroyed?.Invoke();
    }
}

//