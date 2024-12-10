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
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
    private void ClickYes()
    {
        Time.timeScale = 1f;
        FadeManager.Instance.LoadScene("TitleScene");
        //SceneManager.LoadScene("TitleScene");
    }

    private void ClickNo()
    {
        gameObject.SetActive(false);
    }
}
