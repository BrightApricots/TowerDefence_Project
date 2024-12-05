using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : Tower
{
    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private List<GameObject> muzzleEffects;
    [SerializeField] private List<GameObject> HitEffects;

    private GameObject currentProjectile;
    private GameObject currentMuzzleEffect;
    private GameObject currentHitEffect;

    public List<Monster> targetMonsters;
    public bool IsTargeting;
    public int RimitTarget = 10;
    public MissileTower()
    {
        Name = "Missile Tower";
        Element = "Electric";
        Damage = 3;
        Range = 15;
        FireRate = 3f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 65;
        SellPrice = 33;
        TargetPriority = "Most Progress";
        Info = "Each attack fires 10 missiles, targeting random enemies within the attack range.";
    }
    protected override void Start()
    {
        currentProjectile = projectiles[Level - 1];
        currentMuzzleEffect = muzzleEffects[Level - 1];
        currentHitEffect = HitEffects[Level - 1];
        base.Start();
    }

    protected override void Detect()
    {
        targetMonsters = new List<Monster>(); // targetMonsters 리스트 초기화
        if (CurrentTarget == null)
        {
            int count = 0;
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
            foreach (Collider target in hitColliders)
            {
                if (target.CompareTag("Monster"))
                {
                    targetMonsters.Add(target.GetComponent<Monster>());
                    count++;
                    if (count == RimitTarget)
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            if (Vector3.Distance(CurrentTarget.transform.position, transform.position) > Range)
            {
                CurrentTarget = null;
            }
        }
        currentProjectile.GetComponent<MissileProjectile>().SetTargets(targetMonsters);
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate);
            if (targetMonsters.Count != 0)
            {
                foreach (var monster in targetMonsters)
                {
                    yield return new WaitForSeconds(0.05f);
                    GameObject projectile = Instantiate(currentProjectile, TowerMuzzle.transform.position, TowerHead.transform.rotation);
                    Projectile proj = projectile.GetComponent<Projectile>();
                    proj.Damage = this.Damage;
                    proj.IsTargeting = this.IsTargeting;
                    proj.IsBomb = this.IsBomb;
                    proj.Target = monster.transform; // 각 몬스터를 타겟으로 설정

                    
                    GameObject _MuzzleEffect = Instantiate(currentMuzzleEffect, TowerMuzzle.transform.position, TowerHead.transform.rotation);
                    Destroy(_MuzzleEffect, _MuzzleEffect.GetComponent<ParticleSystem>().main.duration);
                }
            }
            targetMonsters.Clear();
            currentProjectile.GetComponent<MissileProjectile>().ClearTargets();
        }
    }

    protected override void OnLevelUp()
    {
        if (Level == 2)
        {
            Damage += 1;
            Range += 1f;
            SetByLevel();
        }
        else if (Level == 3)
        {
            FireRate /= 2f;
            Range += 3f;
            SetByLevel();
        }
    }

    private void SetByLevel()
    {
        currentProjectile = projectiles[Level - 1];
        currentMuzzleEffect = muzzleEffects[Level - 1];
        currentHitEffect = HitEffects[Level - 1];
    }
}