using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LobbyScene : MonoBehaviour
{
    public Button Setting;
    public Button Title;
    public Button TowerLoadout;
    public Button Backpack;
    public Button Talents;
    //public GameObject SettingPopup;
    public GameObject TowerLoadoutPopup;
    public GameObject BackpackPopup;
    //public GameObject TalentsPopup;
    public TextMeshProUGUI EmberAmountText;
    public TextMeshProUGUI ExpAmountText;
    public TextMeshProUGUI HpAmountText;

    private void Awake()
    {
        //Setting.onClick.AddListener(PopupSetting);
        Title.onClick.AddListener(BackToTitle);
        TowerLoadout.onClick.AddListener(PopupTowerLoadout);
        Backpack.onClick.AddListener(PopupBackpack);
        //Talents.onClick.AddListener(PopupTalents);
    }

    private void Update()
    {
        EmberAmountText.text = $"{GameManager.Instance.EmberAmount}";
        ExpAmountText.text = $"{GameManager.Instance.ExpAmount}";
        HpAmountText.text = $"{GameManager.Instance.HpAmount}/15";
    }

    //private void PopupSetting()
    //{ 
    //    SettingPopup.SetActive(true);
    //}
    private void BackToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
    private void PopupTowerLoadout()
    {
        TowerLoadoutPopup.SetActive(true);
    }
    private void PopupBackpack()
    {
        BackpackPopup.SetActive(true);
    }
    //private void PopupTalents()
    //{
    //    TalentsPopup.SetActive(true);
    //}
} 
