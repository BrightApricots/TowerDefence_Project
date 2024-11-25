using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int Cost;
    public string Name;
    public int Damage;
    public float FireRate;
    public float Range;
    public string Info;
    public bool isTargeting;
    public Transform TowerHeadPosition;

    protected virtual void Detect()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
        foreach (Collider target in hitColliders)
        {
            if(target.CompareTag("Enemy"))
            {
                Vector3 fireDir = target.transform.position-TowerHeadPosition.position;

            }
        }
    }


}
