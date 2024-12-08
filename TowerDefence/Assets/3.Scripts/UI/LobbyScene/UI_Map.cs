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
        if(GameManager.Instance.clearStage == 0)
        { 
            SceneManager.LoadScene("AcademyScene");
        }
    }

    private void LoadStage1()
    {
        if(GameManager.Instance.clearStage == 1)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage2()
    {
        if (GameManager.Instance.clearStage == 2)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage3()
    {
        if (GameManager.Instance.clearStage == 3)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage4()
    {
        if (GameManager.Instance.clearStage == 4)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
    private void LoadStage5()
    {
        if (GameManager.Instance.clearStage == 5)
        {
            SceneManager.LoadScene("InGameScene3");
        }
    }
}
