using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Basic : Block
{
    protected override void Awake()
    {
        blockType = 101;
        base.Awake();
    }
}