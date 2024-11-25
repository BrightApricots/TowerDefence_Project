using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerLoadOut : MonoBehaviour
{
    public Button Exit;

    private void Awake()
    {
        Exit.onClick.AddListener(ExitTowerLoadOut);
    }

    private void ExitTowerLoadOut()
    {
        gameObject.SetActive(false);
    }
}
