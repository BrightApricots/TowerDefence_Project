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

    public void Update()
    {
        MasterVolumeRatio.text = $"{(int)MasterVolume.value}%";
        SoundVolumeRatio.text = $"{(int)SoundVolume.value}%";
        MusicVolumeRatio.text = $"{(int)MusicVolume.value}%";
        if (Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}
