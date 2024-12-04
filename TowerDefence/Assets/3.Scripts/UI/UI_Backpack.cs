using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Backpack : MonoBehaviour
{
    public Button Exit;
    public GameObject NoCard;
    public Transform Cards;

    private void Awake()
    {
        Exit.onClick.AddListener(ExitBackpack);
    }
    private void Start()
    {
        if (GameManager.Instance.PlayerTetrisList.Count == 0)
        {
            NoCard.SetActive(true);
        }
        else
        {
            NoCard.SetActive(false);
        }
    }

    private void Update()
    {
        
    }

    private void ExitBackpack()
    {
        gameObject.SetActive(false);
    }
}


