using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;

public class MissileProjectile : Projectile
{
    List<Monster> targetedMosters = new List<Monster>();

    protected override void Bomb(Collider other)
    {
        if (Target.GetComponent<Monster>() == other.GetComponent<Monster>())
        {
            if (other.CompareTag("Monster"))
            {
                Collider[] hit = Physics.OverlapSphere(transform.position, BombRange);

                Instantiate(ExplosionParticle, transform.position, Quaternion.identity);
                foreach (Collider h in hit)
                {
                    if (h.CompareTag("Monster"))
                    {
                        other.gameObject.GetComponent<Monster>().TakeDamage(Damage);
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    public void SetTargets(List<Monster> monsters)
    {
        targetedMosters = monsters;
    }
    public void ClearTargets()
    {
        targetedMosters?.Clear();
    }
}
