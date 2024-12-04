using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PausePanel : MonoBehaviour
{
    public Button Continue;
    public Button Options;
    public Button Title;
    public GameObject OptionsPanel;
    //public GameObject BackTitlePopup;

    private void Awake()
    {
        Continue.onClick.AddListener(EnterContinue);
        Options.onClick.AddListener(EnterOptions);
        //Title.onClick.AddListener(BackToTitle);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }

    private void EnterContinue()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private void EnterOptions()
    {
        OptionsPanel.SetActive(true);
    }

    /*private void BackToTitle()
    {
        BackTitlePopup.SetActive(true);
    }*/
}
