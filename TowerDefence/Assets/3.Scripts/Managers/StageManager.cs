// StageManager.cs는 게임의 상태 전환 및 UI 관리를 담당하는 스크립트
// 주요 기능
// 1. 씬 전환 관리: 다음 스테이지 또는 게임 오버 씬으로 이동.
// 2. 웨이브 상태 관리: 웨이브 완료 시 패널 표시 및 상태 업데이트.
// 3. UI 상태 관리: 특정 상황에서 UI 요소를 활성화/비활성화.
// 4. 이벤트 처리: 웨이브 완료 또는 게임 오버 시 적절한 처리 수행.

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Object nextStageScene;     // 다음 스테이지 씬 정보
    [SerializeField] private Object gameOverScene;      // 게임 오버 씬 정보

    [SerializeField] private GameObject EndPanel;       // 모든 웨이브 클리어 시 표시할 준비 패널
    [SerializeField] private GameObject GameOverPanel;  // 게임 오버 시 표시할 패널
    [SerializeField] private Button gameOverButton;     // 게임 오버 패널의 버튼

    [SerializeField] private List<GameObject> uiElementsToDisable; // 특정 상황에서 비활성화할 UI 요소들

    private WaveManager waveManager;                   // 웨이브 진행 관리
    private bool allWavesCleared = false;              // 모든 웨이브 클리어 여부

    private void Start()
    {
        // 초기화 작업
        InitializeWaveManager();
        InitializePanels();
        InitializeUIElements();
        InitializeGameOverButton();
    }

    private void Update()
    {
        // 게임 오버 및 다음 스테이지 전환 체크
        CheckGameOverCondition();
        CheckNextStageTransition();
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
        }
    }

    private void UnregisterEvents()
    {
        // 등록된 이벤트 해제
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -= HandleAllWavesCleared;
        }

        if (gameOverButton != null)
        {
            gameOverButton.onClick.RemoveListener(OnGameOverButtonClicked);
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

    private void CheckNextStageTransition()
    {
        // 모든 웨이브 클리어 및 클릭 입력 시 다음 스테이지로 이동
        if (allWavesCleared && IsPanelActive(EndPanel) && Input.GetMouseButtonDown(0))
        {
            LoadNextStage();
        }
    }

    private void HandleAllWavesCleared()
    {
        // 모든 웨이브 클리어 처리
        allWavesCleared = true;
        ShowPreparationPanel();        
    }

    private void ShowPreparationPanel()
    {
        // 준비 패널 표시
        SoundManager.Instance.Play("Win_Bgm", SoundManager.Sound.Bgm, 1, false);
        SetPanelActive(EndPanel, true);
        SetUIElementsActive(uiElementsToDisable, false);

        Time.timeScale = 1f;
    }

    private void ShowGameOverPanel()
    {
        // 게임 오버 패널 표시
        SetPanelActive(GameOverPanel, true);
        SetUIElementsActive(uiElementsToDisable, false, gameOverButton?.gameObject);
        Time.timeScale = 0f;
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        // 패널 활성화 상태 설정
        if (panel != null)
        {
            panel.SetActive(isActive);
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
            }
        }
    }

    public void OnGameOverButtonClicked()
    {
        // 게임 오버 버튼 클릭 시 처리
        GameManager.Instance.Clear();
        LoadScene(gameOverScene);
    }

    private void LoadNextStage()
    {
        GameManager.Instance.ClearWin();

        // 다음 스테이지 씬 로드
        if (allWavesCleared)
        {
            GameManager.Instance.clearStage += 1;
            GameManager.Instance.CurrentEmber += 10;
            GameManager.Instance.CurrentExp += 10;
            LoadScene(nextStageScene);
        }
    }

    private void LoadScene(Object scene)
    {
        // 지정된 씬 로드
        if (scene == null) return;

        string sceneName = scene.name;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void OpenGameOverScene()
    {
        // 게임 오버 씬으로 이동
        if (gameOverScene != null)
        {
            string sceneName = gameOverScene.name;
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("게임 오버 씬이 설정되지 않았습니다.");
        }
    }
}
