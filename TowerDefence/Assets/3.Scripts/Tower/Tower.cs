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
    public Transform Barrel;
    public Transform TowerMuzzle;
    public Transform Projectile;
    private Monster CurrentTarget=null;

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
        if (CurrentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, ShootRange);
            foreach (Collider target in hitColliders)
            {
                if (target.CompareTag("Monster"))
                {
                    CurrentTarget =target.GetComponent<Monster>();
                    break;
                }
            }
        }
        else
        {
            if (Vector3.Distance(CurrentTarget.transform.position, transform.position)>ShootRange)
            {
                CurrentTarget=null;
            }
        }
    }

    protected void FollowTarget()
    {
        if(CurrentTarget !=null)
        {
            Vector3 towerDir = CurrentTarget.transform.position - TowerHead.transform.position;
            Barrel.forward = towerDir;
            towerDir.y = 0;
            TowerHead.forward = towerDir;
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootCooltime);
            if (CurrentTarget !=null)
            {
                Transform projectile = Instantiate(Projectile, TowerMuzzle.transform.position,Barrel.transform.rotation);
                projectile.gameObject.GetComponent<Projectile>().Damage = this.Damage;
                projectile.gameObject.GetComponent<Projectile>().IsTargeting = this.IsTargeting;
                projectile.gameObject.GetComponent<Projectile>().IsBomb = this.IsBomb;
                projectile.gameObject.GetComponent<Projectile>().Target = this.CurrentTarget;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ShootRange);
    }
}

//
