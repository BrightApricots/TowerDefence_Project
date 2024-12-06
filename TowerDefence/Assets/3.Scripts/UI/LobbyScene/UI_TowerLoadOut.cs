using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerLoadOut : MonoBehaviour
{
    public Button Exit;
    public Transform EquipTower;
    public Transform UnEquipTower;

    public List<Transform> EquipSlotList;
    public List<Transform> UnEquipSlotList;

    private void Awake()
    {
        Exit.onClick.AddListener(ExitTowerLoadOut);
        Exit.onClick.AddListener(FindObjectOfType<UI_LobbyScene>().TowerCard);
    }

    private void Start()
    {
        for (int i = 0; i<GameManager.Instance.EquipTowerList.Count; i++)
        {
            Instantiate(GameManager.Instance.EquipTowerList[i], EquipSlotList[i]);
        }
        for(int i= 0; i<GameManager.Instance.UnEquipTowerList.Count; i++)
        {
            Instantiate(GameManager.Instance.UnEquipTowerList[i], UnEquipSlotList[i]);
        }
    }

    private void ExitTowerLoadOut()
    {
        gameObject.SetActive(false);
    }
}
