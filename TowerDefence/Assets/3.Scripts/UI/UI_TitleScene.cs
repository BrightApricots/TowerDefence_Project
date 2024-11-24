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
    public GameObject Difficulty;

    private void OnEnable()
    {
        NewGame.onClick.AddListener(LoadNewGame);
        Continue.onClick.AddListener(LoadContinue);
        Exit.onClick.AddListener(LoadExit);
    }

    private void LoadNewGame()
    {
        Difficulty.SetActive(true);
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
