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
            Instantiate(Resources.Load<GameObject>($"TowerLoadoutCard/{GameManager.Instance.EquipTowerList[i].name}"), EquipTower);
        }
        for(int i= 0; i<GameManager.Instance.UnEquipTowerList.Count; i++)
        {
            Instantiate(Resources.Load<GameObject>($"TowerLoadoutCard/{GameManager.Instance.UnEquipTowerList[i].name}"), UnEquipTower);
        }
    }

    private void ExitTowerLoadOut()
    {
        gameObject.SetActive(false);
    }
}
