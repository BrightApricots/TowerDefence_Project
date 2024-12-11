using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private List<Wave> waves = new List<Wave>();      // 전체 웨이브 데이터를 저장하는 리스트
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
    private Slider timeBar;                           // 웨이브 준비 시간 바
    [SerializeField]
    private GameObject speedButtonGroup;              // 배속 버튼 그룹 오브젝트
    [SerializeField]
    private GameObject waveStartText;                 // 웨이브 시작 텍스트 
    [SerializeField]
    private TextMeshProUGUI waveStartStateText;            // 웨이브 시작 상세 텍스트 
    [SerializeField]
    private GameObject waveEndText;                   // 웨이브 클리어 텍스트 
    [SerializeField]
    private TextMeshProUGUI waveEndStateText;              // 웨이브 클리어 상세 텍스트 


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

    private int currentWaveIndex = 0;                 // 현재 웨이브 위치 -> 1단계인지 2단계인지를 나타내는 개념
    private bool isWaveActive = false;                // 현재 웨이브 진행 중 여부, 웨이브 활성화 상태인지 알기 위한 개념
    [SerializeField]
    private int remainingMonsters = 0;                // 남은 몬스터 수
    private bool isReadyForNextWave = false;          // 다음 웨이브 준비 상태 여부
    private bool isGameOver = false;                  // 게임 오버 여부
    private bool isFirstBattleClicked = false;        // 처음 배틀 시작을 눌렀는지 여부
    private int waveClearMoney = 30;                  // 웨이브 클리어 시 지급할 머니
    private Coroutine prepareCooldownCoroutine;       // 준비 시간 코루틴 참조

    public event System.Action OnAllWavesCleared;     // 모든 웨이브 클리어 시 발생하는 이벤트

    [Header("Spawn Settings")]
    [SerializeField]
    private List<Transform> spawnPoints;
    [SerializeField]
    private Transform targetPoint;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject gameClearUI;                    // 게임 클리어 UI
    [SerializeField]
    private GameObject gameOverUI;                     // 게임 오버 UI

    // 추가된 변수들
    private int totalMonstersToSpawn = 0; // 현재 웨이브에서 총 스폰할 몬스터 수
    private int spawnedMonsters = 0;      // 현재까지 스폰된 몬스터 수

    private const int WaveClearEffectsCount = 3; // 매직 넘버 상수화

    private void Start()
    {
        // PathManager 초기화
        if (PathManager.Instance != null)
        {
            PathManager.Instance.SetupPoints(spawnPoints, targetPoint);
        }
        else
        {
            Debug.LogError("PathManager 인스턴스가 존재하지 않습니다!");
        }

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
        else
        {
            Debug.LogError("BattleButton이 할당되지 않았습니다!");
        }
    }

    private void InitializeSpeedButtonGroup()
    {
        if (speedButtonGroup != null)
        {
            speedButtonGroup.SetActive(false);
            foreach (var button in speedButtonGroup.GetComponentsInChildren<Button>())
            {
                button.onClick.AddListener(() =>
                {
                    if (float.TryParse(button.name, out float speed))
                    {
                        ChangeGameSpeed(speed);
                        // 버튼 클릭 시 소리 재생
                        SoundManager.Instance.Play("SpeedChangeSound", SoundManager.Sound.Effect);
                    }
                    else
                    {
                        Debug.LogError($"배속 값 파싱 실패: {button.name}");
                    }
                });
            }
        }
        else
        {
            Debug.LogError("SpeedButtonGroup이 할당되지 않았습니다!");
        }
    }

    private void InitializeWaveTextManager()
    {
        if (waveTextManager != null)
        {
            waveTextManager.Initialize(waves);
            waveTextManager.ShowWaveInfo();
        }
        else
        {
            Debug.LogError("WaveTextManager가 할당되지 않았습니다!");
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
            SoundManager.Instance.Play("SpeedChangeSound", SoundManager.Sound.Effect);
        }
        else if (Input.GetKeyDown(KeyCode.X)) // X키: 배속 1x
        {
            ChangeGameSpeed(1f);
            SoundManager.Instance.Play("SpeedChangeSound", SoundManager.Sound.Effect);
        }
        else if (Input.GetKeyDown(KeyCode.C)) // C키: 배속 2x
        {
            ChangeGameSpeed(2f);
            SoundManager.Instance.Play("SpeedChangeSound", SoundManager.Sound.Effect);
        }
        else if (Input.GetKeyDown(KeyCode.V)) // V키: 배속 3x
        {
            ChangeGameSpeed(3f);
            SoundManager.Instance.Play("SpeedChangeSound", SoundManager.Sound.Effect);
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

            yield return null; // 매 프레임마다 업데이트
            elapsed += Time.deltaTime;
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

        SoundManager.Instance.Play("BattleButton", SoundManager.Sound.Effect);
        StartCoroutine(ShowWaveStateText(waveStartText));

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
        if (monsterSpawner == null)
        {
            Debug.LogError("MonsterSpawner가 할당되지 않았습니다!");
            return;
        }

        SoundManager.Instance.Play("InWave", SoundManager.Sound.Bgm);
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
        monsterSpawner.OnSpawnComplete += OnSpawnComplete; // 추가된 이벤트

        // 총 몬스터 수 계산
        totalMonstersToSpawn = 0;
        foreach (var spawnData in currentWave.monsterSpawnData)
        {
            totalMonstersToSpawn += spawnData.spawnCount;
        }
        spawnedMonsters = 0;
        remainingMonsters = totalMonstersToSpawn; // 변경된 부분: 전체 몬스터 수로 초기화

        monsterSpawner.StartSpawning();
        UpdateWaveText();
    }

    private void OnMonsterSpawned(GameObject monster)
    {
        spawnedMonsters++;
        // remainingMonsters++; // 이 줄을 주석 처리하거나 제거합니다.
        Debug.Log($"Monster Spawned: {spawnedMonsters}, Remaining: {remainingMonsters}");
    }


    private void OnMonsterDestroyed(GameObject monster)
    {
        remainingMonsters--;
        Debug.Log($"Monster Destroyed: Remaining: {remainingMonsters}");

        // 모든 몬스터가 스폰되고 모두 파괴되었을 때 웨이브 종료
        if (remainingMonsters <= 0 && monsterSpawner.IsSpawningComplete())
        {
            EndCurrentWave();
        }
    }


    private void OnSpawnComplete()
    {
        // 모든 몬스터가 스폰된 상태를 표시
        if (spawnedMonsters >= totalMonstersToSpawn && remainingMonsters <= 0)
        {
            EndCurrentWave();
        }
    }

    private void EndCurrentWave()
    {
        // 이벤트 해제
        monsterSpawner.OnMonsterSpawned -= OnMonsterSpawned;
        monsterSpawner.OnMonsterDestroyed -= OnMonsterDestroyed;
        monsterSpawner.OnSpawnComplete -= OnSpawnComplete;

        SoundManager.Instance.Play("Battlefield", SoundManager.Sound.Bgm);

        // 웨이브 클리어 처리
        for (int i = 0; i < WaveClearEffectsCount; i++)
        {
            UI_Draw.draw(); // 매직 넘버 3을 상수로 정의하는 것이 좋습니다.
        }

        GameManager.Instance.CurrentMoney += waveClearMoney;
        waveEndStateText.text = $"Bounus Reward : {waveClearMoney}";
        waveClearMoney += 10;

        isWaveActive = false;
        currentWaveIndex++;
        waveStartStateText.text = $"-WAVE {currentWaveIndex + 1}-";

        if (speedButtonGroup != null) speedButtonGroup.SetActive(false);

        if (currentWaveIndex < waves.Count)
        {
            SoundManager.Instance.Play("WaveClearEffect", SoundManager.Sound.Effect);
            StartCoroutine(ShowWaveStateText(waveEndText));
            isReadyForNextWave = true;
            UpdateWaveText();
            UpdateWavePrepareText();

            if (currentWaveIndex != 0)
            {
                prepareCooldownCoroutine = StartCoroutine(PrepareCooldownRoutine());
            }

            // 다음 웨이브 준비를 위한 UI 업데이트
            waveTextManager?.SetCurrentWaveIndex(currentWaveIndex);
            waveTextManager?.ShowWaveInfo();
        }
        else
        {
            // 모든 웨이브 완료 시 이벤트 호출
            OnAllWavesCleared?.Invoke();
            HandleAllWavesCleared();
        }
    }

    private void HandleAllWavesCleared()
    {
        // 게임 클리어 처리 로직 추가 (예: UI 표시, 보상 지급 등)
        Debug.Log("All waves cleared! Game Cleared!");
        if (gameClearUI != null)
        {
            gameClearUI.SetActive(true); // 게임 클리어 UI 활성화
        }
        Time.timeScale = 0; // 게임 정지
    }

    private void ChangeGameSpeed(float speed)
    {
        if (speed <= 0)
        {
            Debug.LogError("배속 값은 0보다 커야 합니다!");
            return;
        }

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

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // 게임 오버 UI 활성화
        }
    }

    private void OnAllWavesClearedHandler()
    {
        // 추가적인 게임 클리어 로직 구현 가능
        HandleAllWavesCleared();
    }

    private void OnEnable()
    {
        OnAllWavesCleared += OnAllWavesClearedHandler;
    }

    private void OnDisable()
    {
        OnAllWavesCleared -= OnAllWavesClearedHandler;
    }
    
    private IEnumerator ShowWaveStateText(GameObject go)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(2f);
        go.SetActive(false);
        yield return null;
    }
}
