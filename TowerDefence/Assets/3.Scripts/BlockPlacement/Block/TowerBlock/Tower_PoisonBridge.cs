using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_PoisonBridge : Tower_Placement
{
    public int _TowerPrice = 45;
    protected override void Awake()
    {
        blockType = 106;
        //TowerPrice = _TowerPrice;
        base.Awake();
    }
}
