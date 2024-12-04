using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private Object nextStageScene; // 할당할 씬

    [SerializeField]
    private GameObject EndPanel; // 종료 패널 UI

    [SerializeField]
    private List<GameObject> uiElementsToDisable; // 비활성화할 UI 요소들

    private WaveManager waveManager;
    private bool allWavesCleared = false; // 웨이브가 모두 종료되었는지 여부

    private void Start()
    {
        // WaveManager 찾기
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared += HandleAllWavesCleared;
        }
        else
        {
            Debug.LogError("WaveManager를 찾을 수 없습니다.");
        }

        // 준비 패널 초기 비활성화
        if (EndPanel != null)
        {
            EndPanel.SetActive(false);
        }

        // UI 요소 초기 활성화
        foreach (var element in uiElementsToDisable)
        {
            if (element != null)
            {
                element.SetActive(true);
            }
        }
        // 텍스트 초기화
       
        for (int i = 0; i<3; i++)
        {
            UI_Draw.draw();
        }
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        if (waveManager != null)
        {
            waveManager.OnAllWavesCleared -= HandleAllWavesCleared;
        }
    }

    // 모든 웨이브 클리어 시 호출
    private void HandleAllWavesCleared()
    {
        Debug.Log("모든 웨이브가 클리어되었습니다. 준비 패널을 표시합니다.");
        allWavesCleared = true;
        ShowPreparationPanel();
        UI_Map.clearStage+=1;
    }

    // 준비 패널 표시
    private void ShowPreparationPanel()
    {
        if (EndPanel != null)
        {
            EndPanel.SetActive(true);
            Time.timeScale = 1f; // 게임 속도를 1배속으로 설정
        }
        else
        {
            Debug.LogError("준비 패널이 설정되지 않았습니다.");
        }

        // 나머지 UI 요소 비활성화
        foreach (var element in uiElementsToDisable)
        {
            if (element != null)
            {
                element.SetActive(false);
            }
        }
    }

    // 매 프레임마다 호출되어 마우스 클릭을 체크
    private void Update()
    {
        if (allWavesCleared && EndPanel.activeSelf && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            LoadNextStage();
        }
    }

    // 마우스 클릭 시 호출되어 다음 스테이지로 이동
    public void LoadNextStage()
    {
        if (allWavesCleared)
        {
            if (nextStageScene != null)
            {
                string sceneName = nextStageScene.name; // Inspector에서 설정된 씬 이름 가져오기
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("다음 스테이지 씬이 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning("모든 웨이브가 종료되지 않았습니다.");
        }
    }
}

//