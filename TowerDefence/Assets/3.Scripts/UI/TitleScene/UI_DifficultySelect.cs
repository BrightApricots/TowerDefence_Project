using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_DifficultySelect : MonoBehaviour
{
    public Button Casual;
    public Button Normal;
    public Button Heroic;
    public Button BackToTitle;

    private void OnEnable()
    {
        Casual.onClick.AddListener(EnterCasual);
        Normal.onClick.AddListener(EnterNormal);
        Heroic.onClick.AddListener(EnterHeroic);
        BackToTitle.onClick.AddListener(EnterBackToTitle);
    }

    private void EnterCasual()
    {
        GameManager.Instance.Difficulty=1;
        SceneManager.LoadScene("LobbyScene");
    }
    private void EnterNormal()
    {
        GameManager.Instance.Difficulty=2;
        SceneManager.LoadScene("LobbyScene");
    }
    private void EnterHeroic()
    {
        GameManager.Instance.Difficulty=3;
        SceneManager.LoadScene("LobbyScene");
    }
    private void EnterBackToTitle()
    {
        gameObject.SetActive(false);
    }
}
