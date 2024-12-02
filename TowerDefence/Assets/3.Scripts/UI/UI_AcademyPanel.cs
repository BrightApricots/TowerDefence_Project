using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_AcademyPanel : MonoBehaviour
{
    enum Tetris
    {
        Block_L,
        Block_T,
        Block_O,
        Block_I,
        Block_Dot,
        Block_Z
    }

    enum Tower
    {
        Tower_Basic,
        Tower_Fire,
        Tower_Missile
    }

    public GameObject TetrisListLocation;
    public GameObject TowerListLocation;
    private List<GameObject> TetrisList;
    private List<GameObject> TowerList;

    private void Awake()
    {
        
    }

    private void RandomCard()
    {
        for (int i = 0; i < 6; i++)
        {
            int cardnum;
            GameObject go;
            cardnum = Random.Range(0, GameManager.Instance.TotalTetrisList.Count);
            go = Instantiate(Resources.Load<GameObject>(Enum.GetName(typeof(Tetris), cardnum)),TetrisListLocation.transform);
            TetrisList.Add(go);
        }

        for (int i = 0; i < 3; i++)
        {
            int cardnum;
            GameObject go;cardnum = Random.Range(0, GameManager.Instance.TotalTetrisList.Count);
            while(true)
            {

            }
            go = Instantiate(Resources.Load<GameObject>(Enum.GetName(typeof(Tetris), cardnum)), TetrisListLocation.transform);
            TetrisList.Add(go);
        }
    }
}
