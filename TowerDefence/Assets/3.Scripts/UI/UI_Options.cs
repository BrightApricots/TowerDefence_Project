using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    public Slider MasterVolume;
    public Slider SoundVolume;
    public Slider MusicVolume;
    public TextMeshProUGUI MasterVolumeRatio;
    public TextMeshProUGUI SoundVolumeRatio;
    public TextMeshProUGUI MusicVolumeRatio;
    public Button Check;

    private void Awake()
    {
        MasterVolume.value = 50;
        SoundVolume.value = 100;
        MusicVolume.value = 100;
        Check.onClick.AddListener(Confirm);
    }

    private void Update()
    {
        MasterVolumeRatio.text = $"{(int)MasterVolume.value}%";
        SoundVolumeRatio.text = $"{(int)SoundVolume.value}%";
        MusicVolumeRatio.text = $"{(int)MusicVolume.value}%";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    private void Confirm()
    {
        gameObject.SetActive(false);
    }
}
