using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class UI_AcademyPanel : MonoBehaviour, IPointerClickHandler
{
    enum TetrisEnum
    {
        Block_L,
        Block_T,
        Block_O,
        Block_I,
        Block_Dot,
        Block_Z
    }
    
    enum ImageTetrisEnum
    {
        Image_L,
        Image_T,
        Image_O,
        Image_I,
        Image_Dot,
        Image_Z
    }

    enum TowerEnum
    {
        Tower_Basic,
        Tower_Fire,
        Tower_Missile,
        Count
    };

    enum ImageTowerEnum
    {
        Image_Basic,
        Image_Fire,
        Image_Missile,
        Count
    };

    public GameObject TetrisListLocation;
    public GameObject TowerListLocation;
    public List<GameObject> ImageTetrisList;
    public List<GameObject> TetrisList;
    public List<GameObject> ImageTowerList;
    public List<GameObject> TowerList;

    private void Awake()
    {
        RandomTetrisCard();
        RandomTowerCard();
    }

    private void RandomTetrisCard()
    {
        for (int i = 0; i < 7; i++)
        {
            int cardnum;
            GameObject go;
            cardnum = Random.Range(0, GameManager.Instance.TotalTetrisList.Count);
            go = Instantiate(Resources.Load<GameObject>($"TetrisCardPrefab/{Enum.GetName(typeof(ImageTetrisEnum), cardnum)}"),
                TetrisListLocation.transform);
            ImageTetrisList.Add(go);
            TetrisList.Add(Resources.Load<GameObject>($"TetrisCardPrefab/{Enum.GetName(typeof(TetrisEnum), cardnum)}"));
        }
    }

    private void RandomTowerCard()
    {
        int[] tower = new int[(int)TowerEnum.Count];
        for(int i = 0; i<(int)TowerEnum.Count; i++)
        {
            tower[i] = i;
        }
        int random1, random2;
        int temp;
        for(int i= 0; i<=tower.Length; i++)
        {
            random1 = Random.Range(0, tower.Length);
            random2 = Random.Range(0, tower.Length);
            temp = tower[random1];
            tower[random1] = tower[random2];
            tower[random2] = temp;
        }
        GameObject go;
        for (int i = 0; i < 3; i++)
        {
            go = Instantiate(Resources.Load<GameObject>($"TowerCardPrefab/{Enum.GetName(typeof(ImageTowerEnum), tower[i])}"),
                TowerListLocation.transform);
            ImageTowerList.Add(go);
            TowerList.Add(Resources.Load<GameObject>($"TowerCardPrefab/{Enum.GetName(typeof(TowerEnum), tower[i])}"));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.EquipTowerList=TowerList;
        GameManager.Instance.PlayerTetrisList=TetrisList;
        SceneManager.LoadScene("LobbyScene");
    }
}