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
        //TODO : �ҷ����⸦ ����� �� ���� ���� �� ���� ������ ���� ��� �˾�
    }

    private void LoadContinue()
    {
        //TODO : �ҷ�����
    }

    private void LoadExit()
    {
        Application.Quit();
    }
}
