using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public int CurrentExp = 10;
    public int CurrentEmber = 10;
    public int CurrentHp = 15;
    public int CurrentMoney = 50;
    public int MaxHp = 15;
    public int Difficulty;
    public List<int> TotalTetrisList = new List<int> { 0,1,2,3,4,5}; //0~100
    public List<int> TotalTowerList;  //101~200
    public List<GameObject> PlayerTetrisList;
    public List<GameObject> HandTetrisList;
    public List<GameObject> EquipTowerList = new List<GameObject>();
    public List<GameObject> UnEquipTowerList;
    public List<Tower> PlacedTowerList;

    public int clearStage = 0;



    public bool tooltipCount = false;

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

    //재시작시 게임매니저 초기화 메서드
    public void Clear()
    {
        clearStage = 0;
        CurrentHp = 15;
        CurrentMoney = 50;
        MaxHp = 15;

        if (PlayerTetrisList != null)
        {
            foreach (var obj in PlayerTetrisList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            PlayerTetrisList.Clear();
        }

        if (HandTetrisList != null)
        {
            foreach (var obj in HandTetrisList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            HandTetrisList.Clear();
        }

        if (EquipTowerList != null)
        {
            foreach (var obj in EquipTowerList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            EquipTowerList.Clear();
        }

        if (UnEquipTowerList != null)
        {
            foreach (var obj in UnEquipTowerList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            UnEquipTowerList.Clear();
        }

        if (PlacedTowerList != null)
        {
            foreach (var tower in PlacedTowerList)
            {
                if (tower != null)
                    Destroy(tower.gameObject);
            }
            PlacedTowerList.Clear();
        }

        PlayerTetrisList = new List<GameObject>();
        HandTetrisList = new List<GameObject>();
        EquipTowerList = new List<GameObject>();
        UnEquipTowerList = new List<GameObject>();
        PlacedTowerList = new List<Tower>();

        //각 매니저들 초기화
        GridData.Instance.Clear();
        PathManager.Instance.Clear();
        ObjectPlacer.Instance.Clear();
        PathManager.Instance.Clear();
        PlacementSystem.Instance.Clear();
        PoolManager.Instance.Clear();
        ObjectManager.Instance.Clear();
    }

    public void ClearWin()
    {
        if (HandTetrisList != null)
        {
            foreach (var obj in HandTetrisList)
            {
                if (obj != null)
                    Destroy(obj);
            }
            HandTetrisList.Clear();
        }

        if (PlacedTowerList != null)
        {
            foreach (var tower in PlacedTowerList)
            {
                if (tower != null)
                    Destroy(tower.gameObject);
            }
            PlacedTowerList.Clear();
        }

        HandTetrisList = new List<GameObject>();
        PlacedTowerList = new List<Tower>();

        //각 매니저들 초기화
        GridData.Instance.Clear();
        PathManager.Instance.Clear();
        ObjectPlacer.Instance.Clear();
        PathManager.Instance.Clear();
        PlacementSystem.Instance.Clear();
        PoolManager.Instance.Clear();
        ObjectManager.Instance.Clear();
    }
}
