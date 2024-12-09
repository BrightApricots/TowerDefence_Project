// MonsterSpawner.cs는 몬스터 스폰(생성)을 관리하는 스크립트
// 주요 기능
// 1. 몬스터 데이터 설정: 웨이브마다 등장할 몬스터와 스폰 지점, 생성 주기 설정.
// 2. 몬스터 생성 관리: 활성화된 스폰 지점에서 몬스터를 순환 방식으로 생성.
// 3. 가중치 기반 스폰: 몬스터의 등장 확률을 가중치로 반영하여 스폰.
// 4. 이벤트 처리: 몬스터 생성, 파괴, 스폰 완료 시 이벤트 호출.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterSpawnData
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public int spawnCount;           // 몬스터 등장 수량
}

public class MonsterSpawner : MonoBehaviour
{
    private List<MonsterSpawnData> monsterList = new List<MonsterSpawnData>(); // 현재 웨이브의 몬스터 리스트
    private List<Transform> activeSpawnPoints = new List<Transform>();        // 현재 활성화된 스폰 지점
    private float spawnInterval = 0.5f;                                       // 몬스터 생성 주기
    private bool isSpawning = false;                                          // 몬스터 생성 활성화 여부
    private int currentSpawnIndex = 0;                                        // 현재 스폰 지점 인덱스
    private Dictionary<GameObject, int> remainingSpawnCount = new Dictionary<GameObject, int>(); // 남은 몬스터 수량
    private List<GameObject> weightedMonsterPool = new List<GameObject>();    // 가중치 기반 몬스터 풀

    public event System.Action OnSpawnComplete;          // 모든 몬스터 스폰 완료 이벤트
    public event System.Action<GameObject> OnMonsterSpawned;  // 몬스터 생성 이벤트
    public event System.Action<GameObject> OnMonsterDestroyed; // 몬스터 파괴 이벤트

    // 몬스터 수량 초기화
    private void InitializeSpawnCounts()
    {
        remainingSpawnCount.Clear();
        foreach (var spawnData in monsterList)
        {
            if (spawnData.monsterPrefab != null)
            {
                remainingSpawnCount[spawnData.monsterPrefab] = spawnData.spawnCount;
            }
        }
    }

    // 가중치 기반 몬스터 풀 생성
    private void CreateWeightedMonsterPool()
    {
        weightedMonsterPool.Clear();
        foreach (var spawnData in monsterList)
        {
            if (spawnData.monsterPrefab != null && spawnData.spawnCount > 0)
            {
                for (int i = 0; i < spawnData.spawnCount; i++)
                {
                    weightedMonsterPool.Add(spawnData.monsterPrefab);
                }
            }
        }
    }

    // 몬스터 생성
    private void SpawnMonster()
    {
        if (activeSpawnPoints == null || activeSpawnPoints.Count == 0)
        {
            Debug.LogWarning("활성화된 스폰 지점이 없음");
            return;
        }

        // 순환 방식으로 스폰 지점 선택
        currentSpawnIndex = (currentSpawnIndex + 1) % activeSpawnPoints.Count;
        Transform selectedSpawnPoint = activeSpawnPoints[currentSpawnIndex];

        // 등장 확률 기반으로 몬스터 종류 선택
        if (weightedMonsterPool.Count == 0)
        {
            Debug.Log("모든 몬스터 등장 완료");
            StopSpawning();
            return;
        }

        GameObject selectedMonster = weightedMonsterPool[Random.Range(0, weightedMonsterPool.Count)];

        if (remainingSpawnCount.ContainsKey(selectedMonster))
        {
            remainingSpawnCount[selectedMonster]--;
            if (remainingSpawnCount[selectedMonster] <= 0)
            {
                weightedMonsterPool.RemoveAll(m => m == selectedMonster);
            }
        }

        // 몬스터 인스턴스화
        Vector3 spawnPosition = selectedSpawnPoint.position;
        GameObject monsterObj = Instantiate(selectedMonster, spawnPosition, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();

        if (monster != null)
        {
            monster.Initialize(selectedSpawnPoint);
            monster.OnDestroyed += () => OnMonsterDestroyed?.Invoke(monsterObj); // 파괴 시 이벤트 호출
        }

        OnMonsterSpawned?.Invoke(monsterObj); // 생성 시 이벤트 호출
    }

    // 몬스터 생성 루틴
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }

        OnSpawnComplete?.Invoke(); // 모든 몬스터 스폰 완료 이벤트 호출
    }

    // 몬스터 생성 시작
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            currentSpawnIndex = -1;
            StartCoroutine(SpawnRoutine());
        }
    }

    // 몬스터 생성 중지
    public void StopSpawning()
    {
        isSpawning = false;
    }

    // 몬스터 데이터 설정
    public void SetMonsterData(
        List<MonsterSpawnData> newMonsterList,
        float newSpawnInterval,
        List<Transform> activeSpawnPoints)
    {
        monsterList = newMonsterList;
        spawnInterval = newSpawnInterval; // 외부에서 전달받은 생성 간격 설정
        this.activeSpawnPoints = activeSpawnPoints; // 활성화된 스폰 지점 설정
        InitializeSpawnCounts(); // 몬스터 수량 초기화
        CreateWeightedMonsterPool(); // 가중치 풀 생성
    }
}
