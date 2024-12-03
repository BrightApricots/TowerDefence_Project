using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Missile : Block
{
    protected override void Awake()
    {
        blockType = 102;
        base.Awake();
    }
}