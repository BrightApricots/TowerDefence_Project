using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public int CurrentEmber=10;
    public int CurrentExp=10;
    public int CurrentHp=15;
    public int CurrentMoney = 50;
    public int MaxHp = 15;
    public int Difficulty;
    public List<int> TotalTetrisList; //0~100
    public List<int> TotalTowerList;  //101~200
    public List<GameObject> PlayerTetrisList;
    public List<GameObject> PlayerTowerList;
    public List<GameObject> HandTetrisList;
    public List<GameObject> EquipTowerList;
    public List<Tower> PlacedTowerList;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
}