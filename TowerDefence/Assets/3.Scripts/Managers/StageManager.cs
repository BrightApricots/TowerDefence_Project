using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI 요소를 사용하기 위한 네임스페이스

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private Object nextStageScene; // 다음 스테이지로 이동할 씬

    [SerializeField]
    private Object gameOverScene; // 게임 오버 시 로드할 씬

    [SerializeField]
    private GameObject EndPanel; // 스테이지 종료 후 표시할 패널

    [SerializeField]
    private GameObject GameOverPanel; // 게임 오버 시 표시할 패널

    [SerializeField]
    private Button gameOverButton; // 게임 오버 패널 내의 버튼

    [SerializeField]
    private List<GameObject> uiElementsToDisable; // 특정 상황에서 비활성화할 UI 요소 목록

    private WaveManager waveManager; // 웨이브 매니저 스크립트 참조
    private bool allWavesCleared = false; // 모든 웨이브가 종료되었는지 여부를 나타내는 플래그

    private void Start()
    {
        // 각종 초기화 메서드 호출
        InitializeWaveManager(); // 웨이브 매니저 초기화 및 이벤트 등록
        InitializePanels(); // 패널들을 초기 상태로 설정 (비활성화)
        InitializeUIElements(); // UI 요소들을 초기 상태로 설정 (활성화)
        InitializeGameOverButton(); // 게임 오버 버튼에 이벤트 리스너 등록
    }

    private void Update()
    {
        // 게임 오버 조건 확인
        CheckGameOverCondition();
        // 다음 스테이지로 이동할지 여부 확인
        CheckNextStageTransition();
    }

    private void OnDestroy()
    {
        // 이벤트 등록 해제
        UnregisterEvents();
    }

    // 웨이브 매니저를 초기화하고 이벤트를 등록하는 메서드
    private void InitializeWaveManager()
    {
        waveManager = FindObjectOfType<WaveManager>(); // 씬에서 WaveManager 스크립트 찾기
        if (waveManager != null)
        {
            // 모든 웨이브가 종료되었을 때 호출될 이벤트 등록
            waveManager.OnAllWavesCleared += HandleAllWavesCleared;
        }
    }

    // 모든 패널을 비활성화 상태로 초기화하는 메서드
    private void InitializePanels()
    {
        SetPanelActive(EndPanel, false); // 종료 패널 비활성화
        SetPanelActive(GameOverPanel, false); // 게임 오버 패널 비활성화
    }

    // 지정된 UI 요소들을 활성화하는 메서드
    private void InitializeUIElements()
    {
        SetUIElementsActive(uiElementsToDisable, true); // UI 요소들을 활성화
    }

    // 게임 오버 버튼에 클릭 이벤트를 등록하는 메서드
    private void InitializeGameOverButton()
    {
        if (gameOverButton != null)
        {
            // 게임 오버 버튼 클릭 시 호출될 메서드 등록
            gameOverButton.onClick.AddListener(OnGameOverButtonClicked);
        }
    }

    // 이벤트 등록을 해제하는 메서드
    private void UnregisterEvents()
    {
        if (waveManager != null)
        {
            // 웨이브 종료 이벤트 해제
            waveManager.OnAllWavesCleared -= HandleAllWavesCleared;
        }

        if (gameOverButton != null)
        {
            // 게임 오버 버튼 클릭 이벤트 해제
            gameOverButton.onClick.RemoveListener(OnGameOverButtonClicked);
        }
    }

    // 플레이어의 현재 HP를 확인하여 게임 오버 상태를 처리하는 메서드
    private void CheckGameOverCondition()
    {
        if (GameManager.Instance.CurrentHp <= 0) // 플레이어의 HP가 0 이하인지 확인
        {
            ShowGameOverPanel(); // 게임 오버 패널 표시
        }
    }

    // 모든 웨이브가 종료되었을 때 다음 스테이지로 이동할지 확인하는 메서드
    private void CheckNextStageTransition()
    {
        // 모든 웨이브가 종료되었고, EndPanel이 활성화되어 있으며, 마우스 왼쪽 버튼이 클릭되었을 때
        if (allWavesCleared && IsPanelActive(EndPanel) && Input.GetMouseButtonDown(0))
        {
            LoadNextStage(); // 다음 스테이지 로드
        }
    }

    // 모든 웨이브가 종료되었을 때 호출되는 이벤트 핸들러
    private void HandleAllWavesCleared()
    {
        allWavesCleared = true; // 모든 웨이브 종료 플래그 설정
        ShowPreparationPanel(); // 준비 패널 표시
        UI_Map.clearStage += 1; // 클리어한 스테이지 수 증가
    }

    // 준비 패널을 표시하고 UI 요소를 비활성화하는 메서드
    private void ShowPreparationPanel()
    {
        SetPanelActive(EndPanel, true); // 준비 패널 활성화
        PrefabManager.Instance.ReplaceChildWithPrefab(); // PrefabManager를 통해 자식 오브젝트 교체
        SetUIElementsActive(uiElementsToDisable, false); // 지정된 UI 요소들 비활성화
        Time.timeScale = 1f; // 게임 시간 속도를 정상화
    }

    // 게임 오버 패널을 표시하고 UI 요소를 비활성화하는 메서드
    private void ShowGameOverPanel()
    {
        SetPanelActive(GameOverPanel, true); // 게임 오버 패널 활성화
        SetUIElementsActive(uiElementsToDisable, false, gameOverButton?.gameObject); // 게임 오버 버튼을 제외한 UI 요소 비활성화
        Time.timeScale = 0f; // 게임 정지
    }

    // 지정된 패널의 활성 상태를 설정하는 메서드
    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive); // 패널의 활성 상태 설정
        }
    }

    // 지정된 패널이 활성화되어 있는지 확인하는 메서드
    private bool IsPanelActive(GameObject panel)
    {
        return panel != null && panel.activeSelf; // 패널이 null이 아니고 활성화되어 있는지 반환
    }

    // 지정된 UI 요소들의 활성 상태를 설정하는 메서드
    private void SetUIElementsActive(List<GameObject> elements, bool isActive, GameObject excludeElement = null)
    {
        foreach (var element in elements)
        {
            // 제외할 요소가 아니면 활성 상태 설정
            if (element != null && element != excludeElement)
            {
                element.SetActive(isActive);
            }
        }
    }

    // 게임 오버 버튼이 클릭되었을 때 호출되는 메서드
    public void OnGameOverButtonClicked()
    {
        LoadScene(gameOverScene); // 게임 오버 씬 로드
    }

    // 다음 스테이지를 로드하는 메서드
    private void LoadNextStage()
    {
        if (allWavesCleared)
        {
            LoadScene(nextStageScene); // 다음 스테이지 씬 로드
        }
    }

    // 지정된 씬을 로드하는 메서드
    private void LoadScene(Object scene)
    {
        if (scene != null)
        {
            string sceneName = scene.name; // 씬의 이름을 가져옴
            Time.timeScale = 1f; // 게임 시간 속도를 정상화
            SceneManager.LoadScene(sceneName); // 씬 로드
        }
    }
}
