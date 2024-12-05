using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_BackToTitlePanel : MonoBehaviour
{
    public Button Yes;
    public Button No;

    private void Awake()
    {
        Yes.onClick.AddListener(ClickYes);
        No.onClick.AddListener(ClickNo);
    }

    private void ClickYes()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    private void ClickNo()
    {
        gameObject.SetActive(false);
    }
}
