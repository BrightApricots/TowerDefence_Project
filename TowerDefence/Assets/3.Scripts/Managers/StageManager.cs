// StageManager.cs는 게임의 상태 전환 및 UI 관리를 담당하는 스크립트
// 주요 기능
// 1. 씬 전환 관리: 다음 스테이지 또는 게임 오버 씬으로 이동.
// 2. 웨이브 상태 관리: 웨이브 완료 시 패널 표시 및 상태 업데이트.
// 3. UI 상태 관리: 특정 상황에서 UI 요소를 활성화/비활성화.
// 4. 이벤트 처리: 웨이브 완료 또는 게임 오버 시 적절한 처리 수행.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [Header("씬 이름 설정")]
    [SerializeField] private string lobbySceneName = "LobbyScene";       // 로비 씬 이름
    [SerializeField] private string titleSceneName = "TitleScene";       // 타이틀 씬 이름

    [Header("패널 및 버튼 설정")]
    [SerializeField] private GameObject EndPanel;                        // 모든 웨이브 클리어 시 표시할 준비 패널
    [SerializeField] private GameObject GameOverPanel;                   // 게임 오버 시 표시할 패널
    [SerializeField] private Button gameOverButton;                      // 게임 오버 패널의 버튼

    [Header("비활성화할 UI 요소들")]
    [SerializeField] private List<GameObject> uiElementsToDisable;       // 특정 상황에서 비활성화할 UI 요소들

    private Button nextStageButton;                                      // 다음 스테이지 버튼
    private WaveManager waveManager;                                    // 웨이브 진행 관리
    private bool allWavesCleared = false;                               // 모든 웨이브 클리어 여부

    private void Start()
    {
        // 초기화 작업
        InitializeWaveManager();
        InitializePanels();
        InitializeUIElements();
        InitializeGameOverButton();
        InitializeNextStageButton(); // 다음 스테이지 버튼 초기화
    }

    private void Update()
    {
        // 게임 오버 조건 확인
        CheckGameOverCondition();
        // 마우스 클릭 감지 로직 제거
    }

    private void OnDestroy()
    {
        // 이벤트 등록 해제
        UnregisterEvents();
    }

    private void InitializeWaveManager()
    {
        // 웨이브 매니저 초기화 및 이벤트 등록
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared += HandleAllWavesCleared;
            Debug.Log("WaveManager 이벤트 등록 완료.");
        }
        else
        {
            Debug.LogError("WaveManager를 찾을 수 없습니다.");
        }
    }

    private void InitializePanels()
    {
        // 준비 패널과 게임 오버 패널 비활성화
        SetPanelActive(EndPanel, false);
        SetPanelActive(GameOverPanel, false);
    }

    private void InitializeUIElements()
    {
        // UI 요소 활성화 상태 설정
        SetUIElementsActive(uiElementsToDisable, true);
    }

    private void InitializeGameOverButton()
    {
        // 게임 오버 버튼에 클릭 이벤트 등록
        if (gameOverButton != null)
        {
            gameOverButton.onClick.AddListener(OnGameOverButtonClicked);
            Debug.Log("GameOverButton 이벤트 등록 완료.");
        }
        else
        {
            Debug.LogError("GameOverButton이 할당되지 않았습니다.");
        }
    }

    private void InitializeNextStageButton()
    {
        // EndPanel의 자식 중 이름이 "NextStageButton"인 버튼을 찾아 할당
        if (EndPanel != null)
        {
            // EndPanel의 모든 자식 중 Button 컴포넌트를 가진 게임 오브젝트 검색
            Button[] buttons = EndPanel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.name == "NextStageButton")
                {
                    nextStageButton = btn;
                    nextStageButton.onClick.AddListener(LoadNextStage);
                    nextStageButton.gameObject.SetActive(false); // 초기에는 비활성화
                    Debug.Log("'NextStageButton'을 찾았고, 리스너를 추가했습니다.");
                    break;
                }
            }

            if (nextStageButton == null)
            {
                Debug.LogError("EndPanel의 자식에서 'NextStageButton' 이름을 가진 버튼을 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("EndPanel이 할당되지 않았습니다.");
        }
    }

    private void UnregisterEvents()
    {
        // 등록된 이벤트 해제
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -= HandleAllWavesCleared;
            Debug.Log("WaveManager 이벤트 해제 완료.");
        }

        if (gameOverButton != null)
        {
            gameOverButton.onClick.RemoveListener(OnGameOverButtonClicked);
            Debug.Log("GameOverButton 이벤트 해제 완료.");
        }

        if (nextStageButton != null)
        {
            nextStageButton.onClick.RemoveListener(LoadNextStage);
            Debug.Log("NextStageButton 이벤트 해제 완료.");
        }
    }

    private void CheckGameOverCondition()
    {
        // 게임 오버 조건 확인
        if (GameManager.Instance.CurrentHp <= 0)
        {
            ShowGameOverPanel();
        }
    }

    private void HandleAllWavesCleared()
    {
        // 모든 웨이브 클리어 처리
        allWavesCleared = true;
        Debug.Log("모든 웨이브가 클리어되었습니다.");
        ShowPreparationPanel();
    }

    private void ShowPreparationPanel()
    {
        // 준비 패널 표시
        SoundManager.Instance.Play("Win_Bgm", SoundManager.Sound.Bgm, 1, false);
        SetPanelActive(EndPanel, true);
        EndPanel.GetComponent<UI_EndPanel>().init();
        SetUIElementsActive(uiElementsToDisable, false);

        if (nextStageButton != null)
        {
            nextStageButton.gameObject.SetActive(true); // 버튼 활성화
        }
        else
        {
            Debug.LogError("NextStageButton이 할당되지 않았습니다.");
        }

        Time.timeScale = 1f;
        Debug.Log("EndPanel 활성화 및 NextStageButton 표시 완료.");
    }

    private void ShowGameOverPanel()
    {
        // 게임 오버 패널 표시
        SetPanelActive(GameOverPanel, true);
        if (gameOverButton != null)
        {
            SetUIElementsActive(uiElementsToDisable, false, gameOverButton.gameObject);
        }
        else
        {
            SetUIElementsActive(uiElementsToDisable, false);
        }
        Time.timeScale = 0f;
        Debug.Log("GameOverPanel 활성화 완료.");
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        // 패널 활성화 상태 설정
        if (panel != null)
        {
            panel.SetActive(isActive);
            Debug.Log($"{panel.name} 활성화 상태를 {isActive}로 설정했습니다.");
        }
    }

    private bool IsPanelActive(GameObject panel)
    {
        // 패널 활성화 상태 확인
        return panel != null && panel.activeSelf;
    }

    private void SetUIElementsActive(List<GameObject> elements, bool isActive, GameObject excludeElement = null)
    {
        // UI 요소 활성화 상태 설정
        foreach (var element in elements)
        {
            if (element != null && element != excludeElement)
            {
                element.SetActive(isActive);
                Debug.Log($"{element.name} 활성화 상태를 {isActive}로 설정했습니다.");
            }
        }
    }

    public void OnGameOverButtonClicked()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        // 게임 오버 버튼 클릭 시 처리
        Debug.Log("게임 오버 버튼 클릭됨.");
        GameManager.Instance.Clear();
        LoadScene(titleSceneName);
    }

    private void LoadNextStage()
    {
        // 다음 스테이지 씬 로드
        Debug.Log("다음 스테이지 로드 시작.");
        GameManager.Instance.ClearWin();

        if (allWavesCleared)
        {
            GameManager.Instance.clearStage += 1;
            GameManager.Instance.CurrentEmber += 10;
            GameManager.Instance.CurrentExp += 10;
            LoadScene(lobbySceneName);
        }
    }

    private void LoadScene(string sceneName)
    {
        // 지정된 씬 로드
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("로드할 씬 이름이 비어 있습니다.");
            return;
        }

        Debug.Log($"씬 '{sceneName}' 로드 시작.");
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void OpenGameOverScene()
    {
        // 게임 오버 씬으로 이동
        if (!string.IsNullOrEmpty(titleSceneName))
        {
            string sceneName = titleSceneName;
            Debug.Log($"게임 오버 씬 '{sceneName}' 로드 시작.");
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("게임 오버 씬 이름이 설정되지 않았습니다.");
        }
    }
}
