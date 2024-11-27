using UnityEngine;
using System.Collections;
using System.Linq;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject monsterPrefab;
    [SerializeField]
    private float spawnInterval = 2f;
    private bool isSpawning = false;
    private int currentSpawnIndex = 0;

    void Start()
    {
        StartSpawning();
    }

    private void SpawnMonster()
    {
        var spawnPoints = PathManager.Instance.GetSpawnPoints();
        if (spawnPoints == null || spawnPoints.Count == 0) return;

        var validSpawnPoints = spawnPoints.Where(sp => 
        {
            if (sp == null) return false;
            var path = PathManager.Instance.GetCurrentPath(sp);
            return path != null && path.Length > 0;
        }).ToList();

        if (validSpawnPoints.Count == 0) return;

        currentSpawnIndex = (currentSpawnIndex + 1) % validSpawnPoints.Count;
        Transform selectedSpawnPoint = validSpawnPoints[currentSpawnIndex];

        Vector3 spawnPosition = PathManager.Instance.GetSpawnPosition(selectedSpawnPoint);
        GameObject monsterObj = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();
        
        if (monster != null)
        {
            monster.Initialize(selectedSpawnPoint);
        }
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

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            currentSpawnIndex = -1;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
} 