using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
    public Button Stage1;
    public Button Stage2;
    public Button Stage3;
    public Button Stage4;
    public Button Stage5;

    private void Awake()
    {
        Stage1.onClick.AddListener(LoadStage1);
        Stage2.onClick.AddListener(LoadStage1);
        Stage3.onClick.AddListener(LoadStage1);
        Stage4.onClick.AddListener(LoadStage1);
        Stage5.onClick.AddListener(LoadStage1);
    }

    private void LoadStage1()
    {
        SceneManager.LoadScene("InGameScene1");
    }
}
