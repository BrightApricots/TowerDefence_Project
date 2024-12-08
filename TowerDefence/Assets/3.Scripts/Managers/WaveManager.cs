using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame;

[System.Serializable]
public class Wave
{
    public List<MonsterSpawnData> monsterSpawnData;   // 각 웨이브별 몬스터 스폰 데이터 리스트
    public float spawnInterval = 1f;                  // 몬스터 스폰 간격
    public List<Transform> spawnPoints = new List<Transform>(); // 몬스터 스폰 위치 리스트
}

[System.Serializable]
public class SpeedSetting
{
    public float speedMultiplier; // 배속 값 (0.5x, 1x, 2x, 3x 등)
    public List<Image> speedImages; // 해당 배속 상태일 때 활성/비활성 색상을 적용할 이미지들
}

namespace MyGame
{
    [System.Serializable]
    public class MonsterSpawnData
    {
        public GameObject monsterPrefab; // 몬스터 프리팹
        public string monsterName;       // 몬스터 이름
        public int spawnCount;           // 해당 웨이브에서 스폰할 몬스터 수
    }
}

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private List<Wave> waves = new List<Wave>();      // 전체 웨이브 리스트
    [SerializeField]
    private MonsterSpawner monsterSpawner;            // 몬스터 스포너
    [SerializeField]
    private Button battleButton;                      // 배틀 시작 버튼
    [SerializeField]
    private TextMeshProUGUI waveStatusText;           // 현재 웨이브 상태 텍스트 (Ex: Wave 1/5)
    [SerializeField]
    private TextMeshProUGUI wavePrepareText;          // 다음 웨이브 준비 텍스트
    [SerializeField]
    private TextMeshProUGUI wavePrepareTimeText;      // 다음 웨이브까지 남은 시간 안내 텍스트
    [SerializeField]
    private TextMeshProUGUI wavePrepareTimeText_1;    // 남은 초 표시 텍스트
    [SerializeField]
    private Slider timeBar;                  // 웨이브 준비 시간 바
    [SerializeField]
    private GameObject speedButtonGroup;              // 배속 버튼 그룹 오브젝트

    [SerializeField]
    private List<SpeedSetting> speedSettings = new List<SpeedSetting>(); // 배속별 UI 상태
    [SerializeField]
    private Color activeColor = Color.green;          // 활성 배속 색상
    [SerializeField]
    private Color inactiveColor = Color.gray;         // 비활성 배속 색상

    [SerializeField]
    private float prepareCooldown = 10f;              // 웨이브 시작 전 대기 시간(쿨다운)

    [SerializeField]
    private WaveTextManager waveTextManager;          // 웨이브 정보 표시를 관리하는 매니저

    private int currentWaveIndex = 0;                 // 현재 웨이브 인덱스
    private bool isWaveActive = false;                // 현재 웨이브 진행 중 여부
    private int remainingMonsters = 0;                // 남은 몬스터 수
    private bool isReadyForNextWave = false;          // 다음 웨이브 준비 상태 여부
    private bool isGameOver = false;                  // 게임 오버 여부
    private bool isFirstBattleClicked = false;        // 처음 배틀 시작을 눌렀는지 여부
    private int waveClearMoney = 50;                  // 웨이브 클리어 시 지급할 머니
    private Coroutine prepareCooldownCoroutine;       // 준비 시간 코루틴 참조

    public event System.Action OnAllWavesCleared;     // 모든 웨이브 클리어 시 발생하는 이벤트

    private void Start()
    {
        InitializeBattleButton();
        InitializeSpeedButtonGroup();
        InitializeWaveTextManager();
        InitializeUIState();
        UpdateSpeedImageColors(1f); // 초기 배속 1배속 설정
    }

    private void Update()
    {
        // 게임 오버 체크
        if (GameManager.Instance.CurrentHp <= 0 && !isGameOver)
        {
            HandleGameOver();
            return; // 게임오버 처리 후 아래 로직 불필요
        }

        // HP가 0보다 크고 게임 오버 상태가 아닐 때 스페이스바 입력 처리
        if (GameManager.Instance.CurrentHp > 0 && !isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) && battleButton != null && battleButton.interactable)
            {
                battleButton.onClick.Invoke();
            }
        }

        // 웨이브 진행 중일 때만 배속(Z, X, C, V) 키 입력 처리
        if (isWaveActive)
        {
            HandleSpeedInput();
        }
    }

    private void InitializeBattleButton()
    {
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(StartNextWaveManually);
            SetBattleButtonState(true);
        }
    }

    private void InitializeSpeedButtonGroup()
    {
        if (speedButtonGroup != null)
        {
            speedButtonGroup.SetActive(false);
            foreach (var button in speedButtonGroup.GetComponentsInChildren<Button>())
            {
                button.onClick.AddListener(() => ChangeGameSpeed(float.Parse(button.name)));
            }
        }
    }

    private void InitializeWaveTextManager()
    {
        if (waveTextManager != null)
        {
            waveTextManager.Initialize(waves);
            waveTextManager.ShowWaveInfo();
        }
    }

    private void InitializeUIState()
    {
        // 웨이브 준비 시간 관련 UI 비활성화
        if (wavePrepareTimeText != null) wavePrepareTimeText.gameObject.SetActive(false);
        if (wavePrepareTimeText_1 != null) wavePrepareTimeText_1.gameObject.SetActive(false);
        if (timeBar != null) timeBar.gameObject.SetActive(false);

        UpdateWaveText();
        UpdateWavePrepareText();
    }

    // 키보드 입력(Z, X, C, V)에 따른 배속 변경 처리
    private void HandleSpeedInput()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Z키: 배속 0.5x
        {
            ChangeGameSpeed(0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.X)) // X키: 배속 1x
        {
            ChangeGameSpeed(1f);
        }
        else if (Input.GetKeyDown(KeyCode.C)) // C키: 배속 2x
        {
            ChangeGameSpeed(2f);
        }
        else if (Input.GetKeyDown(KeyCode.V)) // V키: 배속 3x
        {
            ChangeGameSpeed(3f);
        }
    }

    // 준비 시간 카운트다운 코루틴
    private IEnumerator PrepareCooldownRoutine()
    {
        float elapsed = 0f;

        SetBattleButtonState(true);

        if (!isFirstBattleClicked && wavePrepareText != null)
        {
            wavePrepareText.gameObject.SetActive(true);
        }

        if (wavePrepareTimeText != null)
        {
            wavePrepareTimeText.gameObject.SetActive(true);
            wavePrepareTimeText.text = "NEXT WAVE IN";
        }

        if (timeBar != null)
        {
            timeBar.gameObject.SetActive(true);
            timeBar.value = 0f;
        }

        while (elapsed < prepareCooldown)
        {
            if (isWaveActive) // 중간에 웨이브 시작 시 즉시 종료
            {
                HideTimeBar();
                yield break;
            }

            if (wavePrepareTimeText_1 != null)
            {
                wavePrepareTimeText_1.text = $"{Mathf.CeilToInt(prepareCooldown - elapsed)}";
                wavePrepareTimeText_1.gameObject.SetActive(true);
            }

            if (timeBar != null)
            {
                timeBar.value = elapsed / prepareCooldown;
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        HideTimeBar();

        if (wavePrepareTimeText_1 != null) wavePrepareTimeText_1.gameObject.SetActive(false);
        if (wavePrepareText != null) wavePrepareText.gameObject.SetActive(false);
        if (wavePrepareTimeText != null) wavePrepareTimeText.gameObject.SetActive(false);

        StartNextWave();
    }

    // 배틀 버튼으로 다음 웨이브 강제 시작
    public void StartNextWaveManually()
    {
        if (isWaveActive || currentWaveIndex >= waves.Count) return;

        if (!isFirstBattleClicked)
        {
            isFirstBattleClicked = true;
            if (wavePrepareText != null) wavePrepareText.gameObject.SetActive(false);
        }

        if (prepareCooldownCoroutine != null)
        {
            StopCoroutine(prepareCooldownCoroutine);
            HideTimeBar();
            prepareCooldownCoroutine = null;
        }

        if (wavePrepareTimeText_1 != null) wavePrepareTimeText_1.gameObject.SetActive(false);
        if (wavePrepareTimeText != null) wavePrepareTimeText.gameObject.SetActive(false);

        StartNextWave();
    }

    private void StartNextWave()
    {
        if (isWaveActive || currentWaveIndex >= waves.Count) return;

        isWaveActive = true;
        isReadyForNextWave = false;
        SetBattleButtonState(false);

        if (speedButtonGroup != null) speedButtonGroup.SetActive(true);

        waveTextManager?.HideWaveInfo();

        var currentWave = waves[currentWaveIndex];
        monsterSpawner.SetMonsterData(currentWave.monsterSpawnData, currentWave.spawnInterval, currentWave.spawnPoints);

        // 이벤트 등록
        monsterSpawner.OnMonsterSpawned += OnMonsterSpawned;
        monsterSpawner.OnMonsterDestroyed += OnMonsterDestroyed;

        monsterSpawner.StartSpawning();
        UpdateWaveText();
    }

    private void OnMonsterSpawned(GameObject monster)
    {
        remainingMonsters++;
    }

    private void OnMonsterDestroyed(GameObject monster)
    {
        remainingMonsters--;

        if (remainingMonsters <= 0 && isWaveActive)
        {
            EndCurrentWave();
        }
    }

    private void EndCurrentWave()
    {
        // 이벤트 해제
        monsterSpawner.OnMonsterSpawned -= OnMonsterSpawned;
        monsterSpawner.OnMonsterDestroyed -= OnMonsterDestroyed;

        // 웨이브 클리어 처리
        for (int i = 0; i < 3; i++)
        {
            UI_Draw.draw();
        }

        GameManager.Instance.CurrentMoney += waveClearMoney;
        waveClearMoney += 10;

        isWaveActive = false;
        currentWaveIndex++;

        if (speedButtonGroup != null) speedButtonGroup.SetActive(false);

        if (currentWaveIndex < waves.Count)
        {
            isReadyForNextWave = true;
            UpdateWaveText();
            UpdateWavePrepareText();

            if (currentWaveIndex != 0)
            {
                prepareCooldownCoroutine = StartCoroutine(PrepareCooldownRoutine());
            }
        }
        else
        {
            OnAllWavesCleared?.Invoke();
        }

        if (currentWaveIndex < waves.Count)
        {
            isReadyForNextWave = true;
            waveTextManager?.SetCurrentWaveIndex(currentWaveIndex);
            waveTextManager?.ShowWaveInfo();
        }
        else
        {
            OnAllWavesCleared?.Invoke();
        }
    }

    private void ChangeGameSpeed(float speed)
    {
        Time.timeScale = speed;
        UpdateSpeedImageColors(speed);
    }

    private void UpdateSpeedImageColors(float activeSpeed)
    {
        foreach (var setting in speedSettings)
        {
            if (setting.speedImages != null)
            {
                foreach (var speedImage in setting.speedImages)
                {
                    speedImage.color = Mathf.Approximately(setting.speedMultiplier, activeSpeed)
                        ? activeColor
                        : inactiveColor;
                }
            }
        }
    }

    private void HideTimeBar()
    {
        if (timeBar != null) timeBar.gameObject.SetActive(false);
    }

    private void SetBattleButtonState(bool isActive)
    {
        if (battleButton != null) battleButton.gameObject.SetActive(isActive);
    }

    private void UpdateWaveText()
    {
        if (waveStatusText != null)
        {
            if (currentWaveIndex == waves.Count - 1)
            {
                waveStatusText.text = "Final Wave";
            }
            else
            {
                waveStatusText.text = $"Wave {currentWaveIndex + 1}/{waves.Count}";
            }
        }
    }

    private void UpdateWavePrepareText()
    {
        if (wavePrepareText != null)
        {
            wavePrepareText.text = $"Prepare for {currentWaveIndex + 1}st wave!";
        }
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        isWaveActive = false;
        isReadyForNextWave = false;
        SetBattleButtonState(false);
        if (speedButtonGroup != null) speedButtonGroup.SetActive(false);
        Time.timeScale = 0; // 게임 정지
    }
}

// 최적화 처리