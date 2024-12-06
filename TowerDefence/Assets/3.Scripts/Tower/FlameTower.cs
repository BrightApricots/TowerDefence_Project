using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTower: Tower
{
    [SerializeField] private List<GameObject> flameProjeciles;
    private GameObject currentFlameProjectile;

    public bool IsTargeting;

    public FlameTower()
    {
        Name = "Flame Tower";
        Element = "Fire";
        Damage = 2;
        Range = 3.5f;
        FireRate = 2.5f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 30;
        SellPrice = 15;
        TargetPriority = "Most Progress";
        Info = "Continuously spews flames at its target, roasting a large group of monsters to ashes!";
    }

    protected override void Start()
    {
        currentFlameProjectile = flameProjeciles[Level - 1];
        base.Start();
    }

    protected override void Update()
    {
        Detect();
        FollowTarget();
        TooltipPopupCheck();
    }


    protected override IEnumerator Attack()
    {
        while (true)
        {
            if (CurrentTarget != null)
            {
                currentFlameProjectile.SetActive(true);
                FlameProjectile proj = currentFlameProjectile.GetComponent<FlameProjectile>();
                proj.Damage = this.Damage;
                proj.IsTargeting = this.IsTargeting;
                proj.Target = this.CurrentTarget;
                // FireRate 동안 활성화 유지
                yield return new WaitForSeconds(FireRate);
            }
            else
            {
                currentFlameProjectile.SetActive(false);
                yield return null; // 타겟이 없을 때는 다음 프레임까지 대기
            }
        }
    }


    protected override void OnLevelUp()
    {
        if (Level == 2)
        {
            StopCoroutine(Attack());
            currentFlameProjectile.SetActive(false);
            currentFlameProjectile = flameProjeciles[Level - 1];
            StartCoroutine(Attack());
            Damage += 3;
        }
        else if (Level == 3)
        {
            StopCoroutine(Attack());
            currentFlameProjectile.SetActive(false);
            currentFlameProjectile = flameProjeciles[Level - 1];
            StartCoroutine(Attack());
            Damage += 1;
            Range += 2f;
        }
    }
}
