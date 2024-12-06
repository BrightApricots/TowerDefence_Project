using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : Tower
{
    [Header("타워 레벨효과")]
    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private List<GameObject> muzzleEffects;
    [SerializeField] private List<GameObject> HitEffects;

    private GameObject currentProjectile;
    private GameObject currentMuzzleEffect;
    private GameObject currentHitEffect;

    public List<Monster> targetMonsters;
    public bool IsTargeting;
    public bool IsBomb;
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
        targetMonsters = new List<Monster>();
        base.Start();
    }

    protected override void Detect()
    {
        targetMonsters.Clear();
        
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
        foreach (Collider target in hitColliders)
        {
            if (target.CompareTag("Monster"))
            {
                Monster monster = target.GetComponent<Monster>();
                if (monster != null && monster.gameObject.activeSelf)
                {
                    targetMonsters.Add(monster);
                    if (targetMonsters.Count >= RimitTarget)
                    {
                        break;
                    }
                }
            }
        }
        
        CurrentTarget = targetMonsters.Count > 0 ? targetMonsters[0].transform : null;
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate);
            
            targetMonsters.RemoveAll(monster => 
                monster == null || !monster.gameObject.activeSelf);
            
            if (targetMonsters.Count > 0)
            {
                foreach (var monster in targetMonsters.ToArray())
                {
                    if (monster != null && monster.gameObject.activeSelf)
                    {
                        yield return new WaitForSeconds(0.05f);
                        Projectile projectile = ObjectManager.Instance.Spawn<Projectile>(
                            currentProjectile, 
                            TowerMuzzle.transform.position
                        );
                        
                        projectile.transform.rotation = TowerHead.transform.rotation;
                        projectile.Damage = this.Damage;
                        projectile.IsTargeting = this.IsTargeting;
                        projectile.IsBomb = this.IsBomb;
                        projectile.Target = monster.transform;
                        projectile.Initialize();

                        PooledParticle muzzleEffect = ObjectManager.Instance.Spawn<PooledParticle>(
                            currentMuzzleEffect, 
                            TowerMuzzle.transform.position, 
                            TowerHead.transform.rotation
                        );
                        muzzleEffect?.Initialize();
                    }
                }
            }
            
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