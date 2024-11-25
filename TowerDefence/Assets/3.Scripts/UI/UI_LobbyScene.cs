using System.Collections;
using System.Collections.Generic;
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
    public GameObject SettingPopup;
    public GameObject TowerLoadoutPopup;
    public GameObject BackpackPopup;
    public GameObject TalentsPopup;

    private void Awake()
    {
        Setting.onClick.AddListener(PopupSetting);
        Title.onClick.AddListener(BackToTitle);
        TowerLoadout.onClick.AddListener(PopupTowerLoadout);
        Backpack.onClick.AddListener(PopupBackpack);
        Talents.onClick.AddListener(PopupTalents);
    }

    private void PopupSetting()
    { 
        SettingPopup.SetActive(true);
    }
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
    private void PopupTalents()
    {
        TalentsPopup.SetActive(true);
    }
}
