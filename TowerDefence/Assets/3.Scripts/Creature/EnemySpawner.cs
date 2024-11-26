using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 적 프리팹
    public Transform targetBase;   // 적이 이동할 목표 기지의 Transform
    public Transform spawnTransform; // 생성 위치를 나타내는 Transform
    public float spawnInterval = 3.0f; // 적 생성 간격 (초 단위)

    private Pathfinding_AstarMove pathfinding; // A* 경로 탐색을 위한 Pathfinding 스크립트 참조

    private void Start()
    {
        // Pathfinding 스크립트 초기화
        pathfinding = FindObjectOfType<Pathfinding_AstarMove>();
        if (pathfinding == null)
        {
            // Pathfinding 스크립트가 없으면 오류 출력
            Debug.LogError("씬에 Pathfinding 스크립트가 없습니다. 적절한 GameObject에 Pathfinding 스크립트를 추가하세요.");
            return;
        }

        // 일정한 시간 간격으로 적 생성
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && targetBase != null && spawnTransform != null)
        {
            // 적 프리팹을 생성
            GameObject enemyObj = Instantiate(enemyPrefab, spawnTransform.position, Quaternion.identity);
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // 적 초기화: 목표 기지와 Pathfinding 전달
                enemyScript.Initialize(targetBase, pathfinding);
            }
        }
        else
        {
            // 필요한 설정값이 누락된 경우 오류 출력
            Debug.LogError("EnemyPrefab, TargetBase 또는 SpawnTransform이 EnemySpawner에 할당되지 않았습니다.");
        }
    }
}
