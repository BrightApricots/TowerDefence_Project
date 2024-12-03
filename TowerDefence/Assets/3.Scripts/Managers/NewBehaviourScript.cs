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

//[System.Serializable]
//public class SpeedSetting
//{
//    public float speedMultiplier; // 배속 값
//    public Image speedImage;      // 배속과 관련된 이미지
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
//    private List<SpeedSetting> speedSettings = new List<SpeedSetting>(); // 배속 설정 리스트
//    [SerializeField]
//    private Color activeColor = Color.green; // 활성 상태 색상
//    [SerializeField]
//    private Color inactiveColor = Color.gray; // 비활성 상태 색상
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











//        UpdateSpeedImageColors(1f); // 기본 배속 1배속으로 초기화
//    }




//    private void Update()
//    {
//        if (GameManager.Instance.CurrentHp <= 0 && !isGameOver)
//        {
//            HandleGameOver();
//        }

//        HandleSpeedInput(); // 키보드 입력에 따른 배속 처리
//    }






//    private void HandleSpeedInput()
//    {
//        if (Input.GetKeyDown(KeyCode.Z)) // 배속 0.5x
//        {
//            ChangeGameSpeed(0.5f);
//        }
//        else if (Input.GetKeyDown(KeyCode.X)) // 배속 1x
//        {
//            ChangeGameSpeed(1f);
//        }
//        else if (Input.GetKeyDown(KeyCode.C)) // 배속 2x
//        {
//            ChangeGameSpeed(2f);
//        }
//        else if (Input.GetKeyDown(KeyCode.V)) // 배속 3x
//        {
//            ChangeGameSpeed(3f);
//        }
//    }






//    private void ChangeGameSpeed(float speed)
//    {
//        Time.timeScale = speed;
//        UpdateSpeedImageColors(speed); // 배속 변경 시 버튼 아이콘 색상 업데이트
//    }









//    private void UpdateSpeedImageColors(float activeSpeed)
//    {
//        foreach (var setting in speedSettings)
//        {
//            if (setting.speedImage != null)
//            {
//                setting.speedImage.color = Mathf.Approximately(setting.speedMultiplier, activeSpeed)
//                    ? activeColor
//                    : inactiveColor;
//            }
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

//    private void HideTimeBar()
//    {
//        if (timeBarBackground != null) timeBarBackground.gameObject.SetActive(false);
//        if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);
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
