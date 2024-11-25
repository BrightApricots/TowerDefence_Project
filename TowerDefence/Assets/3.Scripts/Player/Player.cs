using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Hp;

    void Start()
    {
        GameManager.Instance.player = this;
    }
}
