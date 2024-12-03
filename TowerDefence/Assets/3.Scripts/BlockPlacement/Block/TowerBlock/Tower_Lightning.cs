using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Lightning : Tower_Placement
{
    public int _TowerPrice = 20;
    protected override void Awake()
    {
        blockType = 104;
        TowerPrice = _TowerPrice;
        base.Awake();
    }
}