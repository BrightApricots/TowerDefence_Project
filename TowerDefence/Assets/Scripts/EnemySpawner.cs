using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 생성할 적 프리팹
    public Transform targetBase;  // 플레이어 기지의 Transform
    public float spawnInterval = 3.0f; // 적 생성 간격

    // 고정된 생성 위치 설정
    private Vector3 spawnPosition = new Vector3(-12f, 1f, -9f);

    private void Start()
    {
        // 일정 주기로 적 생성
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // 적 생성
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // 생성된 적에 플레이어 기지의 위치를 설정
                enemyScript.Initialize(targetBase);
            }
        }
    }
}
