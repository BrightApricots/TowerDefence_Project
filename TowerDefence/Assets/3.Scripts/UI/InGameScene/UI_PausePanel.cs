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
    public GameObject BackTitlePopup;

    private void Awake()
    {
        Continue.onClick.AddListener(EnterContinue);
        Options.onClick.AddListener(EnterOptions);
        Title.onClick.AddListener(BackToTitle);
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
        // 버튼 소리
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    private void EnterOptions()
    {
        // 버튼 소리
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        OptionsPanel.SetActive(true);
    }

    private void BackToTitle()
    {
        // 버튼 소리
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        BackTitlePopup.SetActive(true);
    }
}
