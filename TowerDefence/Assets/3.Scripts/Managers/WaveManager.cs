using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스

[System.Serializable]
public class Wave
{
    public List<MonsterSpawnData> monsterSpawnData; // 웨이브의 몬스터 데이터
    public float spawnInterval = 1f; // 웨이브별 몬스터 생성 간격
    public List<Transform> spawnPoints = new List<Transform>(); // 웨이브별 활성화할 스폰 지점
}

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private List<Wave> waves = new List<Wave>(); // 웨이브 리스트
    [SerializeField]
    private MonsterSpawner monsterSpawner; // MonsterSpawner 참조

    private Button battleButton; // Battle 버튼
    private int currentWaveIndex = 0; // 현재 진행 중인 웨이브 인덱스
    private bool isWaveActive = false; // 웨이브 진행 여부
    private int remainingMonsters = 0; // 남은 몬스터 수
    private bool isReadyForNextWave = false; // 다음 웨이브 시작 여부
    private bool isGameOver = false; // 게임 오버 상태 확인용 변수

    private void Start()
    {
        // Battle 버튼 설정
        GameObject buttonObject = GameObject.Find("Battle");
        if (buttonObject != null)
        {
            battleButton = buttonObject.GetComponent<Button>();
            if (battleButton != null)
            {
                battleButton.onClick.AddListener(PrepareNextWave); // 버튼 클릭 이벤트 연결
                battleButton.gameObject.SetActive(true); // 버튼 활성화
            }
            else
            {
                Debug.LogError("Battle 버튼에서 Button 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Battle 버튼을 찾을 수 없습니다.");
        }
    }

    private void Update()
    {
        // 게임 오버 상태 확인
        if (GameManager.Instance.CurrentHp <= 0 && !isGameOver)
        {
            HandleGameOver();
        }
    }

    // 웨이브 준비
    public void PrepareNextWave()
    {
        if (isWaveActive)
        {
            Debug.Log("현재 웨이브가 진행 중입니다.");
            return;
        }

        Debug.Log($"{currentWaveIndex + 1} 웨이브 ");
        isReadyForNextWave = true;

        if (battleButton != null)
        {
            battleButton.gameObject.SetActive(false); // 버튼 비활성화
        }

        StartNextWave(); // 웨이브 시작
    }

    // 웨이브 시작
    private void StartNextWave()
    {
        if (!isReadyForNextWave)
        {
            Debug.Log("Battle 버튼을 눌러 웨이브를 시작하세요.");
            return;
        }

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("모든 웨이브가 완료되었습니다!");
            return;
        }

        isWaveActive = true;
        isReadyForNextWave = false;

        var currentWave = waves[currentWaveIndex];
        var activeSpawnPoints = GetActiveSpawnPoints(currentWave);

        monsterSpawner.SetMonsterData(
            currentWave.monsterSpawnData,
            currentWave.spawnInterval,
            activeSpawnPoints
        );

        monsterSpawner.OnMonsterSpawned += OnMonsterSpawned;
        monsterSpawner.OnMonsterDestroyed += OnMonsterDestroyed;
        monsterSpawner.StartSpawning();
    }

    // 활성화된 스폰 지점 필터링
    private List<Transform> GetActiveSpawnPoints(Wave currentWave)
    {
        List<Transform> activePoints = new List<Transform>();
        foreach (var point in currentWave.spawnPoints)
        {
            if (point != null) activePoints.Add(point);
        }
        return activePoints;
    }

    // 몬스터 생성 시 호출
    private void OnMonsterSpawned(GameObject monster)
    {
        remainingMonsters++;
    }

    // 몬스터 파괴 시 호출
    private void OnMonsterDestroyed(GameObject monster)
    {
        remainingMonsters--;

        if (remainingMonsters <= 0 && isWaveActive)
        {
            EndCurrentWave(); // 웨이브 종료
        }
    }

    // 웨이브 종료
    private void EndCurrentWave()
    {
        isWaveActive = false;
        currentWaveIndex++;

        if (currentWaveIndex < waves.Count)
        {
            Debug.Log($"{currentWaveIndex} 웨이브 완료. 다음 웨이브 준비 중...");
            isReadyForNextWave = true;

            if (battleButton != null)
            {
                battleButton.gameObject.SetActive(true); // 버튼 활성화
            }
        }
        else
        {
            Debug.Log("모든 웨이브를 클리어했습니다. 축하합니다!");
        }
    }

    // 게임 오버 처리
    private void HandleGameOver()
    {
        Debug.Log("게임 오버.");
        isGameOver = true;
        isWaveActive = false;
        isReadyForNextWave = false;

        if (battleButton != null)
        {
            battleButton.gameObject.SetActive(false); // 버튼 비활성화
        }

        Time.timeScale = 0; // 게임 중단
    }
}
