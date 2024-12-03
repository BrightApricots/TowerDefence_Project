using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Wave
{
    public List<MonsterSpawnData> monsterSpawnData;
    public float spawnInterval = 1f;
    public List<Transform> spawnPoints = new List<Transform>();
}

[System.Serializable]
public class SpeedSetting
{
    public float speedMultiplier; // 배속 값
    public List<Image> speedImages; // 배속과 관련된 여러 이미지들
}

public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private List<Wave> waves = new List<Wave>();
    [SerializeField]
    private MonsterSpawner monsterSpawner;
    [SerializeField]
    private Button battleButton;
    [SerializeField]
    private TextMeshProUGUI waveStatusText;
    [SerializeField]
    private TextMeshProUGUI wavePrepareText;
    [SerializeField]
    private TextMeshProUGUI wavePrepareTimeText;
    [SerializeField]
    private TextMeshProUGUI wavePrepareTimeText_1;
    [SerializeField]
    private Image timeBarBackground;
    [SerializeField]
    private Image timeBarFill;
    [SerializeField]
    private GameObject speedButtonGroup; // 배속 버튼 그룹을 담을 GameObject

    [SerializeField]
    private List<SpeedSetting> speedSettings = new List<SpeedSetting>(); // 배속 설정 리스트
    [SerializeField]
    private Color activeColor = Color.green; // 활성 상태 색상
    [SerializeField]
    private Color inactiveColor = Color.gray; // 비활성 상태 색상

    [SerializeField]
    private float prepareCooldown = 10f;

    private int currentWaveIndex = 0;
    private bool isWaveActive = false;
    private int remainingMonsters = 0;
    private bool isReadyForNextWave = false;
    private bool isGameOver = false;
    private bool isFirstBattleClicked = false;
    private Coroutine prepareCooldownCoroutine;

    public event System.Action OnAllWavesCleared;

    private void Start()
    {
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(StartNextWaveManually);
            SetBattleButtonState(true);
        }

        if (speedButtonGroup != null)
        {
            speedButtonGroup.SetActive(false); // 배속 버튼 그룹 초기 비활성화
            foreach (var button in speedButtonGroup.GetComponentsInChildren<Button>())
            {
                button.onClick.AddListener(() => ChangeGameSpeed(float.Parse(button.name))); // 버튼 이름을 배속 값으로 사용
            }
        }

        UpdateWaveText();
        UpdateWavePrepareText();

        if (wavePrepareTimeText != null) wavePrepareTimeText.gameObject.SetActive(false);
        if (wavePrepareTimeText_1 != null) wavePrepareTimeText_1.gameObject.SetActive(false);
        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(false);
        if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);

        UpdateSpeedImageColors(1f); // 기본 배속 1배속으로 초기화
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentHp <= 0 && !isGameOver)
        {
            HandleGameOver();
        }

        HandleSpeedInput(); // 키보드 입력에 따른 배속 처리
    }

    //키보드 입력에 따른 배속 처리
    private void HandleSpeedInput()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // 1번 키로 배속 0.5x
        {
            ChangeGameSpeed(0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.X)) // 2번 키로 배속 2x
        {
            ChangeGameSpeed(1f);
        }
        else if (Input.GetKeyDown(KeyCode.C)) // 3번 키로 배속 3x
        {
            ChangeGameSpeed(2f);
        }
        else if (Input.GetKeyDown(KeyCode.V)) // 4번 키로 배속 3x로 초기화
        {
            ChangeGameSpeed(3f);
        }
    }


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

        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(true);
        if (timeBarFill != null)
        {
            timeBarFill.gameObject.SetActive(true);
            timeBarFill.fillAmount = 0f;
        }

        while (elapsed < prepareCooldown)
        {
            if (isWaveActive)
            {
                HideTimeBar();
                yield break;
            }

            if (wavePrepareTimeText_1 != null)
            {
                wavePrepareTimeText_1.text = $"{Mathf.CeilToInt(prepareCooldown - elapsed)}";
                wavePrepareTimeText_1.gameObject.SetActive(true);
            }

            if (timeBarFill != null)
            {
                timeBarFill.fillAmount = elapsed / prepareCooldown;
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

        var currentWave = waves[currentWaveIndex];
        monsterSpawner.SetMonsterData(currentWave.monsterSpawnData, currentWave.spawnInterval, currentWave.spawnPoints);

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
    }

    private void ChangeGameSpeed(float speed)
    {
        Time.timeScale = speed;
        UpdateSpeedImageColors(speed); // 배속 변경 시 버튼 아이콘 색상 업데이트
    }

    private void UpdateSpeedImageColors(float activeSpeed)
    {
        foreach (var setting in speedSettings)
        {
            if (setting.speedImages != null)
            {
                foreach (var speedImage in setting.speedImages) // 리스트의 각 Image에 대해 반복문을 실행
                {
                    // 배속 값에 따라 색상을 변경
                    speedImage.color = Mathf.Approximately(setting.speedMultiplier, activeSpeed)
                        ? activeColor  // 배속 값이 활성 배속 값과 일치하면 활성 색상
                        : inactiveColor; // 그렇지 않으면 비활성 색상
                }
            }
        }
    }

    private void HideTimeBar()
    {
        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(false);
        if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);
    }

    private void SetBattleButtonState(bool isActive)
    {
        if (battleButton != null) battleButton.gameObject.SetActive(isActive);
    }

    private void UpdateWaveText()
    {
        if (waveStatusText != null)
        {
            if (currentWaveIndex == waves.Count - 1)  // 마지막 웨이브
            {
                waveStatusText.text = "Final Wave";  // "Final Wave"를 한 줄로 출력
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
        Time.timeScale = 0;
    }
}

//중간완료