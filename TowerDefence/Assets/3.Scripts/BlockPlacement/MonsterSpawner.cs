using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject monsterPrefab;
    [SerializeField]
    private float spawnInterval = 2f;
    private bool isSpawning = false;

    void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            if (PathManager.Instance != null && PathManager.Instance.HasValidPath)
            {
                SpawnMonster();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        Vector3 spawnPosition = PathManager.Instance.GetSpawnPosition();
        GameObject monsterObj = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();
        
        if (monster != null)
        {
            monster.Initialize(PathManager.Instance.GetCurrentPath());
        }
    }
} 