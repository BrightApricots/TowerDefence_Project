using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;
    private Vector3[] path;
    private int currentWaypointIndex;
    private bool isMoving = false;
    private Vector3 currentTargetPosition;
    private Vector3 moveDirection;
    private Transform spawnPoint;

    public void Initialize(Transform spawn)
    {
        spawnPoint = spawn;
        transform.position = PathManager.Instance.GetSpawnPosition(spawnPoint);
        Vector3[] newPath = PathManager.Instance.GetCurrentPath(spawnPoint);
        
        if (newPath != null && newPath.Length > 0)
        {
            path = newPath;
            currentWaypointIndex = 0;
            isMoving = true;
            
            if (PathManager.Instance != null)
            {
                PathManager.Instance.OnPathUpdated += OnPathUpdated;
            }
            
            UpdateCurrentTarget();
        }
        else
        {
            Debug.LogWarning("No valid path found for monster initialization");
        }
    }

    private void OnPathUpdated(Transform updatedSpawnPoint, Vector3[] newPath)
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

    private void UpdateCurrentTarget()
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

        Vector3 targetDirection = (currentTargetPosition - transform.position).normalized;
        moveDirection = Vector3.Lerp(moveDirection, targetDirection, Time.deltaTime * 10f);
        
        transform.position += moveDirection * speed * Time.deltaTime;

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
                    if (PathManager.Instance != null)
                    {
                        PathManager.Instance.OnPathUpdated -= OnPathUpdated;
                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (PathManager.Instance != null)
        {
            PathManager.Instance.OnPathUpdated -= OnPathUpdated;
        }
    }
} 