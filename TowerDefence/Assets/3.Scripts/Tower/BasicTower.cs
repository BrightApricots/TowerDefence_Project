using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Tower
{
    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private List<GameObject> muzzleEffects;
    [SerializeField] private List<GameObject> HitEffects;

    private GameObject currentProjectile;
    private GameObject currentMuzzleEffect;
    private GameObject currentHitEffect;

    public bool IsTargeting;

    public BasicTower()
    {
        Name = "Basic Tower";
        Element = "Normal";
        Damage = 2;
        Range = 4f;
        FireRate = 1.5f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 10;
        SellPrice = 5;
        TargetPriority = "Most Progress";
        Info = "The basic defense tower.";
    }

    protected override void Start()
    {
        currentProjectile = projectiles[Level-1];
        currentMuzzleEffect = muzzleEffects[Level-1];
        currentHitEffect = HitEffects[Level-1];
        base.Start();
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate);
            if (CurrentTarget != null)
            {
                GameObject projectile = Instantiate(currentProjectile, TowerMuzzle.transform.position, TowerHead.transform.rotation);
                Projectile proj = projectile.gameObject.GetComponent<Projectile>();
                proj.Damage = this.Damage;
                proj.IsTargeting = this.IsTargeting;
                proj.IsBomb = this.IsBomb;
                proj.Target = this.CurrentTarget;

                GameObject _MuzzlEffect = Instantiate(currentMuzzleEffect, TowerMuzzle.transform.position, TowerHead.transform.rotation);
                Destroy(_MuzzlEffect, _MuzzlEffect.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }

    protected override void OnLevelUp()
    {
        if (Level == 2)
        {
            Damage += 2;
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
        currentProjectile = projectiles[Level-1];
        currentMuzzleEffect = muzzleEffects[Level-1];
        currentHitEffect = HitEffects[Level-1];
    }
}
