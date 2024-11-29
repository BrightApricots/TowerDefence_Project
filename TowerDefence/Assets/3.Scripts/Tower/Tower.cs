using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Tower : MonoBehaviour
{
    public int Cost;
    public string Name;
    public int Damage;
    public float ShootCooltime=1f;
    public float ShootRange;
    public string Info;
    public bool IsTargeting;
    public bool IsBomb;
    public Transform TowerHead;
    public Transform TowerMuzzle;
    public Transform Bullet;
    private Monster currentTarget =null;

    private void Start()
    {
        GameManager.Instance.PlacedTowerList.Add(this);
        StartCoroutine(Attack());
    }

    private void Update()
    {
        Detect();
        FollowTarget();
    }

    protected void Detect()
    {
        if (currentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, ShootRange);
            foreach (Collider target in hitColliders)
            {
                if (target.CompareTag("Monster"))
                {
                    currentTarget =target.GetComponent<Monster>();
                    break;
                }
            }
        }
        else
        {
            if (Vector3.Distance(currentTarget.transform.position, transform.position)>ShootRange)
            {
                currentTarget=null;
            }
        }
    }

    protected void FollowTarget()
    {
        if(currentTarget !=null)
        {
            Vector3 towerDir = currentTarget.transform.position - TowerHead.transform.position;
            towerDir.y = 0;
            TowerHead.forward = towerDir;
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootCooltime);
            if (currentTarget !=null)
            {
                Instantiate(Bullet, TowerMuzzle.transform.position,TowerHead.transform.rotation);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ShootRange);
    }

}

//
