using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ �� ������
    public Transform targetBase;  // �÷��̾� ������ Transform
    public float spawnInterval = 3.0f; // �� ���� ����

    // ������ ���� ��ġ ����
    private Vector3 spawnPosition = new Vector3(-12f, 1f, -9f);

    private void Start()
    {
        // ���� �ֱ�� �� ����
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // �� ����
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // ������ ���� �÷��̾� ������ ��ġ�� ����
                enemyScript.Initialize(targetBase);
            }
        }
    }
}
