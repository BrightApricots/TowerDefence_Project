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
        // 이름이 "Battle"인 버튼 찾기
        GameObject buttonObject = GameObject.Find("Battle");
        if (buttonObject != null)
        {
            battleButton = buttonObject.GetComponent<Button>();

            if (battleButton != null)
            {
                battleButton.onClick.AddListener(PrepareNextWave); // 버튼 클릭 이벤트에 PrepareNextWave 연결
                battleButton.gameObject.SetActive(true); // 버튼 활성화
            }
            else
            {
                Debug.LogError("Battle 버튼에서 Button 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Battle 버튼을 찾을 수 없습니다. 이름을 확인하세요.");
        }

        Debug.Log("웨이브를 시작하려면 Battle 버튼을 눌러주세요!");
    }

    private void Update()
    {
        // 클리어 실패 조건 확인
        if (GameManager.Instance.CurrentHp <= 0 && !isGameOver)
        {
            HandleGameOver();
        }
    }

    // 버튼에서 호출할 함수
    public void PrepareNextWave()
    {
        if (isWaveActive)
        {
            Debug.Log("현재 웨이브가 진행 중입니다. 웨이브가 종료된 후 시작할 수 있습니다.");
            return;
        }

        Debug.Log("다음 웨이브 준비 완료!");
        isReadyForNextWave = true;

        if (battleButton != null)
        {
            battleButton.gameObject.SetActive(false); // 웨이브 시작 후 버튼 비활성화
        }

        StartNextWave(); // 웨이브 시작
    }

    private void StartNextWave()
    {
        if (!isReadyForNextWave)
        {
            Debug.Log("다음 웨이브 준비 상태가 아닙니다. Battle 버튼을 눌러 웨이브를 시작하세요.");
            return;
        }

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("모든 웨이브가 완료되었습니다!");
            return;
        }

        Debug.Log($"{currentWaveIndex + 1} / 웨이브 시작!");
        isWaveActive = true;
        isReadyForNextWave = false;

        // 현재 웨이브의 데이터를 MonsterSpawner에 전달
        var currentWave = waves[currentWaveIndex];
        var activeSpawnPoints = GetActiveSpawnPoints(currentWave);

        monsterSpawner.SetMonsterData(
            currentWave.monsterSpawnData,
            currentWave.spawnInterval,
            activeSpawnPoints
        );

        // 몬스터 스포너에서 몬스터 생성 시작
        monsterSpawner.OnMonsterSpawned += OnMonsterSpawned; // 몬스터 생성 시 이벤트
        monsterSpawner.OnMonsterDestroyed += OnMonsterDestroyed; // 몬스터 파괴 시 이벤트

        monsterSpawner.StartSpawning();
    }

    private List<Transform> GetActiveSpawnPoints(Wave currentWave)
    {
        // 활성화된 스폰 지점을 필터링
        List<Transform> activePoints = new List<Transform>();
        foreach (var point in currentWave.spawnPoints)
        {
            if (point != null) activePoints.Add(point);
        }
        return activePoints;
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
        Debug.Log($"{currentWaveIndex + 1} / 웨이브 완료!");

        isWaveActive = false;
        currentWaveIndex++;

        // 다음 웨이브 준비 가능 상태로 전환
        if (currentWaveIndex < waves.Count)
        {
            Debug.Log("다음 웨이브를 시작하려면 Battle 버튼을 눌러주세요!");
            isReadyForNextWave = true;

            if (battleButton != null)
            {
                battleButton.gameObject.SetActive(true); // 다음 웨이브 준비 상태에서 버튼 활성화
            }
        }
        else
        {
            Debug.Log("모든 웨이브를 완료했습니다!");
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("클리어 실패: HP가 0입니다!");
        isGameOver = true; // 게임 오버 상태로 전환
        isWaveActive = false;
        isReadyForNextWave = false;

        if (battleButton != null)
        {
            battleButton.gameObject.SetActive(false);
        }

        Time.timeScale = 0; // 게임 중단
    }
}

//