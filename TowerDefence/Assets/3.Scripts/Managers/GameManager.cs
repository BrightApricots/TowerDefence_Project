using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public int CurrentEmber=10;
    public int CurrentExp=10;
    public int CurrentHp=15;
    public int CurrentMoney = 50;
    public int MaxHp = 15;
    public int Difficulty;
    public List<int> TetrisCardList = new List<int> { 0,1,2,3,4,5 };
    public List<GameObject> TetrisHandCardList;
    public List<int> TowerCardList = new List<int> { 6 };
    public List<Tower> PlacedTowerList;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//[System.Serializable]
//public class Wave
//{
//    public List<MonsterSpawnData> monsterSpawnData;
//    public float spawnInterval = 1f;
//    public List<Transform> spawnPoints = new List<Transform>();
//}

//public class WaveManager : MonoBehaviour
//{
//    [SerializeField]
//    private List<Wave> waves = new List<Wave>();
//    [SerializeField]
//    private MonsterSpawner monsterSpawner;
//    [SerializeField]
//    private Button battleButton;
//    [SerializeField]
//    private TextMeshProUGUI waveStatusText;
//    [SerializeField]
//    private TextMeshProUGUI wavePrepareText;
//    [SerializeField]
//    private TextMeshProUGUI additionalText1;
//    [SerializeField]
//    private TextMeshProUGUI additionalText2;
//    [SerializeField]
//    private Image timeBarBackground;
//    [SerializeField]
//    private Image timeBarFill;
//    [SerializeField]
//    private GameObject speedButtonGroup; // 배속 버튼 그룹을 담을 GameObject
//    [SerializeField]
//    private float prepareCooldown = 10f;

//    private int currentWaveIndex = 0;
//    private bool isWaveActive = false;
//    private int remainingMonsters = 0;
//    private bool isReadyForNextWave = false;
//    private bool isGameOver = false;
//    private bool isFirstBattleClicked = false;
//    private Coroutine prepareCooldownCoroutine;

//    public event System.Action OnAllWavesCleared;

//    private void Start()
//    {
//        if (battleButton != null)
//        {
//            battleButton.onClick.AddListener(StartNextWaveManually);
//            SetBattleButtonState(true);
//        }

//        if (speedButtonGroup != null)
//        {
//            speedButtonGroup.SetActive(false); // 배속 버튼 그룹 초기 비활성화
//            foreach (var button in speedButtonGroup.GetComponentsInChildren<Button>())
//            {
//                button.onClick.AddListener(() => ChangeGameSpeed(float.Parse(button.name))); // 버튼 이름을 배속 값으로 사용
//            }
//        }

//        UpdateWaveText();
//        UpdateWavePrepareText();

//        if (additionalText1 != null) additionalText1.gameObject.SetActive(false);
//        if (additionalText2 != null) additionalText2.gameObject.SetActive(false);
//        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(false);
//        if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);
//    }

//    private void Update()
//    {
//        if (GameManager.Instance.CurrentHp <= 0 && !isGameOver)
//        {
//            HandleGameOver();
//        }

//        HandleSpeedInput(); // 키보드 입력에 따른 배속 처리
//    }

//    // 키보드 입력에 따른 배속 처리
//    private void HandleSpeedInput()
//    {
//        if (Input.GetKeyDown(KeyCode.Z)) // 1번 키로 배속 0.5x
//        {
//            ChangeGameSpeed(0.5f);
//        }
//        else if (Input.GetKeyDown(KeyCode.X)) // 2번 키로 배속 2x
//        {
//            ChangeGameSpeed(1f);
//        }
//        else if (Input.GetKeyDown(KeyCode.C)) // 3번 키로 배속 3x
//        {
//            ChangeGameSpeed(2f);
//        }
//        else if (Input.GetKeyDown(KeyCode.V)) // 4번 키로 배속 3x로 초기화
//        {
//            ChangeGameSpeed(3f);
//        }
//    }


//    private IEnumerator PrepareCooldownRoutine()
//    {
//        float elapsed = 0f;

//        SetBattleButtonState(true);

//        if (!isFirstBattleClicked && wavePrepareText != null)
//        {
//            wavePrepareText.gameObject.SetActive(true);
//        }

//        if (additionalText1 != null)
//        {
//            additionalText1.gameObject.SetActive(true);
//            additionalText1.text = "NEXT WAVE IN";
//        }

//        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(true);
//        if (timeBarFill != null)
//        {
//            timeBarFill.gameObject.SetActive(true);
//            timeBarFill.fillAmount = 0f;
//        }

//        while (elapsed < prepareCooldown)
//        {
//            if (isWaveActive)
//            {
//                HideTimeBar();
//                yield break;
//            }

//            if (additionalText2 != null)
//            {
//                additionalText2.text = $"{Mathf.CeilToInt(prepareCooldown - elapsed)}";
//                additionalText2.gameObject.SetActive(true);
//            }

//            if (timeBarFill != null)
//            {
//                timeBarFill.fillAmount = elapsed / prepareCooldown;
//            }

//            yield return new WaitForSeconds(0.1f);
//            elapsed += 0.1f;
//        }

//        HideTimeBar();

//        if (additionalText2 != null) additionalText2.gameObject.SetActive(false);
//        if (wavePrepareText != null) wavePrepareText.gameObject.SetActive(false);
//        if (additionalText1 != null) additionalText1.gameObject.SetActive(false);

//        StartNextWave();
//    }

//    public void StartNextWaveManually()
//    {
//        if (isWaveActive || currentWaveIndex >= waves.Count) return;

//        if (!isFirstBattleClicked)
//        {
//            isFirstBattleClicked = true;
//            if (wavePrepareText != null) wavePrepareText.gameObject.SetActive(false);
//        }

//        if (prepareCooldownCoroutine != null)
//        {
//            StopCoroutine(prepareCooldownCoroutine);
//            HideTimeBar();
//            prepareCooldownCoroutine = null;
//        }

//        if (additionalText2 != null) additionalText2.gameObject.SetActive(false);
//        if (additionalText1 != null) additionalText1.gameObject.SetActive(false);

//        StartNextWave();
//    }

//    private void StartNextWave()
//    {
//        if (isWaveActive || currentWaveIndex >= waves.Count) return;

//        isWaveActive = true;
//        isReadyForNextWave = false;
//        SetBattleButtonState(false);

//        if (speedButtonGroup != null) speedButtonGroup.SetActive(true);

//        var currentWave = waves[currentWaveIndex];
//        monsterSpawner.SetMonsterData(currentWave.monsterSpawnData, currentWave.spawnInterval, currentWave.spawnPoints);

//        monsterSpawner.OnMonsterSpawned += OnMonsterSpawned;
//        monsterSpawner.OnMonsterDestroyed += OnMonsterDestroyed;

//        monsterSpawner.StartSpawning();
//        UpdateWaveText();
//    }

//    private void OnMonsterSpawned(GameObject monster)
//    {
//        remainingMonsters++;
//    }

//    private void OnMonsterDestroyed(GameObject monster)
//    {
//        remainingMonsters--;

//        if (remainingMonsters <= 0 && isWaveActive)
//        {
//            EndCurrentWave();
//        }
//    }

//    private void EndCurrentWave()
//    {
//        isWaveActive = false;
//        currentWaveIndex++;

//        if (speedButtonGroup != null) speedButtonGroup.SetActive(false);

//        if (currentWaveIndex < waves.Count)
//        {
//            isReadyForNextWave = true;
//            UpdateWaveText();
//            UpdateWavePrepareText();

//            if (currentWaveIndex != 0)
//            {
//                prepareCooldownCoroutine = StartCoroutine(PrepareCooldownRoutine());
//            }
//        }
//        else
//        {
//            OnAllWavesCleared?.Invoke();
//        }
//    }

//    private void ChangeGameSpeed(float speed)
//    {
//        Time.timeScale = speed;
//    }

//    private void HideTimeBar()
//    {
//        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(false);
//        if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);
//    }

//    private void SetBattleButtonState(bool isActive)
//    {
//        if (battleButton != null) battleButton.gameObject.SetActive(isActive);
//    }

//    private void UpdateWaveText()
//    {
//        if (waveStatusText != null)
//        {
//            waveStatusText.text = $"Wave {currentWaveIndex + 1}/{waves.Count}";
//        }
//    }

//    private void UpdateWavePrepareText()
//    {
//        if (wavePrepareText != null)
//        {
//            wavePrepareText.text = $"Prepare for {currentWaveIndex + 1}st wave!";
//        }
//    }

//    private void HandleGameOver()
//    {
//        isGameOver = true;
//        isWaveActive = false;
//        isReadyForNextWave = false;
//        SetBattleButtonState(false);
//        if (speedButtonGroup != null) speedButtonGroup.SetActive(false);
//        Time.timeScale = 0;
//    }
//}

//중간완성