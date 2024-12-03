using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
    public static int clearStage = 0;
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
        Stage2.onClick.AddListener(LoadStage1);
        Stage3.onClick.AddListener(LoadStage1);
        Stage4.onClick.AddListener(LoadStage1);
        Stage5.onClick.AddListener(LoadStage1);
    }

    private void EnterAcademy()
    {
        if(clearStage == 0)
        SceneManager.LoadScene("AcademyScene");
    }

    private void LoadStage1()
    {
        if(clearStage == 1)
        SceneManager.LoadScene("InGameScene1");
    }
    private void LoadStage2()
    {
        if (clearStage == 2)
            SceneManager.LoadScene("InGameScene1");
    }
    private void LoadStage3()
    {
        if (clearStage == 3)
            SceneManager.LoadScene("InGameScene1");
    }
    private void LoadStage4()
    {
        if (clearStage == 4)
            SceneManager.LoadScene("InGameScene1");
    }
    private void LoadStage5()
    {
        if (clearStage == 5)
            SceneManager.LoadScene("InGameScene1");
    }
}
