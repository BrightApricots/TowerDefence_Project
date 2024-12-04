using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerLoadOut : MonoBehaviour
{
    public Button Exit;
    public Transform EquipTower;
    public Transform UnEquipTower;

    private void Awake()
    {
        Exit.onClick.AddListener(ExitTowerLoadOut);
    }

    private void Start()
    {
        for (int i = 0; i<GameManager.Instance.EquipTowerList.Count; i++)
        {
            Instantiate(GameManager.Instance.EquipTowerList[i], EquipTower);
        }
        for(int i= 0; i<GameManager.Instance.UnEquipTowerList.Count; i++)
        {
            Instantiate(GameManager.Instance.UnEquipTowerList[i], UnEquipTower);
        }
    }

    private void ExitTowerLoadOut()
    {
        gameObject.SetActive(false);
    }
}
