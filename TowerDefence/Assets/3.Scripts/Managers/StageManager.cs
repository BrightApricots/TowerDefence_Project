using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위한 네임스페이스

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private Object nextStageScene; // Inspector에서 할당할 씬 (UnityEngine.Object 사용)

    private WaveManager waveManager;

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
        Debug.Log("다음 스테이지로 이동합니다.");
        LoadNextStage();
    }

    // 다음 스테이지 씬 로드
    private void LoadNextStage()
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
}

//