using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public List<MonsterSpawnData> monsterSpawnData; // 웨이브의 몬스터 데이터
}

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private List<Wave> waves = new List<Wave>(); // 웨이브 리스트
    [SerializeField]
    private MonsterSpawner monsterSpawner; // MonsterSpawner 참조
    [SerializeField]
    private float waveInterval = 5f; // 웨이브 간 대기 시간

    private int currentWaveIndex = 0; // 현재 진행 중인 웨이브 인덱스
    private bool isWaveActive = false; // 웨이브 진행 여부
    private int remainingMonsters = 0; // 남은 몬스터 수

    private void Start()
    {
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("모든 웨이브가 완료되었습니다!");
            return;
        }

        Debug.Log($"웨이브 {currentWaveIndex + 1} 시작!");
        isWaveActive = true;

        // 현재 웨이브의 몬스터 데이터 설정
        var currentWave = waves[currentWaveIndex];
        monsterSpawner.SetMonsterData(currentWave.monsterSpawnData);

        // 몬스터 스포너에서 몬스터 생성 시작
        monsterSpawner.OnMonsterSpawned += OnMonsterSpawned; // 몬스터 생성 시 이벤트
        monsterSpawner.OnMonsterDestroyed += OnMonsterDestroyed; // 몬스터 파괴 시 이벤트

        monsterSpawner.StartSpawning();
    }

    private void OnMonsterSpawned(GameObject monster)
    {
        remainingMonsters++; // 생성된 몬스터 수 증가
    }

    private void OnMonsterDestroyed(GameObject monster)
    {
        remainingMonsters--; // 파괴된 몬스터 수 감소

        // 남은 몬스터가 없으면 웨이브 종료
        if (remainingMonsters <= 0 && isWaveActive)
        {
            EndCurrentWave();
        }
    }

    private void EndCurrentWave()
    {
        Debug.Log($"웨이브 {currentWaveIndex + 1} 완료!");

        isWaveActive = false;
        currentWaveIndex++;

        // 다음 웨이브 시작
        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(StartNextWaveWithDelay());
        }
        else
        {
            Debug.Log("모든 웨이브를 완료했습니다!");
        }
    }

    private IEnumerator StartNextWaveWithDelay()
    {
        yield return new WaitForSeconds(waveInterval);
        StartNextWave();
    }
}

// 중간 완료