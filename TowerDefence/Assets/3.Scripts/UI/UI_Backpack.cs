using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Backpack : MonoBehaviour
{
    public Button Exit;

    private void Awake()
    {
        Exit.onClick.AddListener(ExitBackpack);
    }

    private void ExitBackpack()
    {
        gameObject.SetActive(false);
    }
}


