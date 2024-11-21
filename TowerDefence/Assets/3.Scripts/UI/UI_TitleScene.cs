using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleScene : MonoBehaviour
{
    public Button NewGame;
    public Button Continue;
    public Button Exit;

    private void Awake()
    {
        NewGame.onClick.AddListener(LoadNewGame);
        Continue.onClick.AddListener(LoadContinue);
        Exit.onClick.AddListener(LoadExit);
    }

    private void LoadNewGame()
    {
        SceneManager.LoadScene("InGameScene");
        //TODO : 불러오기를 만들면 새 게임 시작 시 저장 데이터 삭제 경고 팝업
    }

    private void LoadContinue()
    {
        //TODO : 불러오기
    }

    private void LoadExit()
    {
        Application.Quit();
    }
}
