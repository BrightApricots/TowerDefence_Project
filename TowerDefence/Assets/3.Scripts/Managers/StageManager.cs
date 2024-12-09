// UnityEngine 네임스페이스와 UI 관련 기능 포함
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

    private WaveManager waveManager;   // 웨이브 진행 관리
    private bool allWavesCleared = false; // 모든 웨이브 클리어 여부

    private void Start()
    {
        InitializeWaveManager();   // 웨이브 매니저 초기화 및 이벤트 등록
        InitializePanels();        // 초기 패널 비활성화
        InitializeUIElements();    // UI 요소 초기화
        InitializeGameOverButton(); // 게임 오버 버튼 초기화
    }

    private void Update()
    {
        CheckGameOverCondition();  // HP 상태 확인 후 게임 오버 처리
        CheckNextStageTransition(); // 웨이브 클리어 후 마우스 클릭 시 다음 스테이지로 이동
    }

    private void OnDestroy()
    {
        UnregisterEvents(); // 이벤트 등록 해제
    }

    private void InitializeWaveManager()
    {
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared += HandleAllWavesCleared; // 웨이브 종료 이벤트 등록
        }
    }

    private void InitializePanels()
    {
        SetPanelActive(EndPanel, false);    // 준비 패널 비활성화
        SetPanelActive(GameOverPanel, false); // 게임 오버 패널 비활성화
    }

    private void InitializeUIElements()
    {
        SetUIElementsActive(uiElementsToDisable, true); // UI 요소 초기화 (활성화 상태)
    }

    private void InitializeGameOverButton()
    {
        if (gameOverButton != null)
        {
            gameOverButton.onClick.AddListener(OnGameOverButtonClicked); // 버튼 클릭 시 이벤트 등록
        }
    }

    private void UnregisterEvents()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -= HandleAllWavesCleared; // 웨이브 종료 이벤트 해제
        }

        if (gameOverButton != null)
        {
            gameOverButton.onClick.RemoveListener(OnGameOverButtonClicked); // 버튼 클릭 이벤트 해제
        }
    }

    private void CheckGameOverCondition()
    {
        if (GameManager.Instance.CurrentHp <= 0) // 플레이어 HP 확인
        {
            ShowGameOverPanel(); // HP 0 이하일 경우 게임 오버 처리
        }
    }

    private void CheckNextStageTransition()
    {
        if (allWavesCleared && IsPanelActive(EndPanel) && Input.GetMouseButtonDown(0))
        {
            LoadNextStage(); // 웨이브 클리어 상태에서 클릭 시 다음 스테이지 로드
        }
    }

    private void HandleAllWavesCleared()
    {
        allWavesCleared = true; // 웨이브 클리어 상태 갱신
        ShowPreparationPanel(); // 준비 패널 표시
        GameManager.Instance.clearStage += 1; // 클리어된 스테이지 카운트 증가
    }

    private void ShowPreparationPanel()
    {
        SetPanelActive(EndPanel, true); // 준비 패널 활성화
        GameManager.Instance.ClearWin();
        GameManager.Instance.clearStage++;
        PrefabManager.Instance.ReplaceChildWithPrefab(); // 프리팹 업데이트
        SetUIElementsActive(uiElementsToDisable, false); // UI 비활성화
        Time.timeScale = 1f; // 게임 속도 복구
    }

    private void ShowGameOverPanel()
    {
        SetPanelActive(GameOverPanel, true); // 게임 오버 패널 활성화
        SetUIElementsActive(uiElementsToDisable, false, gameOverButton?.gameObject); // 특정 UI 제외하고 비활성화
        Time.timeScale = 0f; // 게임 정지
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive); // 패널 활성화/비활성화 설정
        }
    }

    private bool IsPanelActive(GameObject panel)
    {
        return panel != null && panel.activeSelf; // 패널 활성 상태 확인
    }

    private void SetUIElementsActive(List<GameObject> elements, bool isActive, GameObject excludeElement = null)
    {
        foreach (var element in elements)
        {
            if (element != null && element != excludeElement)
            {
                element.SetActive(isActive); // 특정 요소 제외하고 UI 상태 변경
            }
        }
    }

    public void OnGameOverButtonClicked()
    {
        GameManager.Instance.Clear();
        LoadScene(gameOverScene); // 게임 오버 버튼 클릭 시 게임 오버 씬 로드
    }

    private void LoadNextStage()
    {
        if (allWavesCleared)
        {
            LoadScene(nextStageScene); // 다음 스테이지 씬 로드
        }
    }

    private void LoadScene(Object scene)
    {
        if (scene == null) return;
        
        string sceneName = scene.name; // 씬 이름 가져오기
        Time.timeScale = 1f; // 타임스케일 복구
        SceneManager.LoadScene(sceneName); // 씬 로드
    }

    // 게임 오버 버튼에 할당할 메서드 추가
    public void OpenGameOverScene()
    {
        if (gameOverScene != null)
        {
            string sceneName = gameOverScene.name; // Inspector에서 설정된 씬 이름 가져오기
            Time.timeScale = 1f; // 씬 전환 전 게임 속도 정상화
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("게임 오버 씬이 설정되지 않았습니다.");
        }
    }
}
