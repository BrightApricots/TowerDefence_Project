using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_AcademyScene : MonoBehaviour
{
    public GameObject Pannel1;
    public GameObject Pannel2;
    public GameObject Pannel3;

    private void Awake()
    {
        Pannel1.GetComponent<Button>().onClick.AddListener(ClickPannel1);
        Pannel2.GetComponent<Button>().onClick.AddListener(ClickPannel2);
        Pannel3.GetComponent<Button>().onClick.AddListener(ClickPannel3);
    }

    private void ClickPannel1()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    private void ClickPannel2()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    private void ClickPannel3()
    {
        SceneManager.LoadScene("LobbyScene");
    }

}
