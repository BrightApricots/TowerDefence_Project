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
    private List<Enemy> targetList;

    protected virtual void Fire()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
        for(int i = 0; i<hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Enemy"))
            {
                targetList[i] = hitColliders[i].gameObject.GetComponent<Enemy>();
            }
        }
    }


}
