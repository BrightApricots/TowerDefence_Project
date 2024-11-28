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
    public List<int> TetrisCardList = new List<int> { 0,1,2,3,4,5 };
    public List<GameObject> TetrisHandCardList;
    public List<int> TowerCardList = new List<int> { 6 };
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
