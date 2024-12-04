using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Placement : Block
{
   
    public int TowerPrice;
    protected override void Placement()
    {
        if (TowerPrice <= GameManager.Instance.CurrentMoney)
        {
            PlacementSystem.Instance.StartPlacement(blockType);
            // GameManager.Instance.CurrentMoney -= TowerPrice;
        }
    }
}
