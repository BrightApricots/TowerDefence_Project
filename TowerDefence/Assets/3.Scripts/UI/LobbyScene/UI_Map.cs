using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
    
    public Button Academy;
    public Button Stage1;
    public Button Stage2;
    public Button Stage3;
    public Button Stage4;
    public Button Stage5;

    private void Awake()
    {
        Academy.onClick.AddListener(EnterAcademy);
        Stage1.onClick.AddListener(LoadStage1);
        Stage2.onClick.AddListener(LoadStage2);
        Stage3.onClick.AddListener(LoadStage3);
        Stage4.onClick.AddListener(LoadStage4);
        Stage5.onClick.AddListener(LoadStage5);
    }
    
    private void EnterAcademy()
    {
         SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        if(GameManager.Instance.clearStage == 0)
        { 
            SceneManager.LoadScene("AcademyScene");
        }
    }

    private void LoadStage1()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        if (GameManager.Instance.clearStage == 1)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage2()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        if (GameManager.Instance.clearStage == 2)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage3()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        if (GameManager.Instance.clearStage == 3)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage4()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        if (GameManager.Instance.clearStage == 4)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage5()
    {
        SoundManager.Instance.Play("Click18", SoundManager.Sound.Effect);
        if (GameManager.Instance.clearStage == 5)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
}
