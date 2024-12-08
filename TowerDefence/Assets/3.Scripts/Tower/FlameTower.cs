using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTower: Tower
{
    [SerializeField] private List<GameObject> flameProjeciles;
    private GameObject currentFlameProjectile;

    public bool IsTargeting;
    private int originalDamage;
    private float originalRange;

    protected override void Start()
    {
        originalDamage = Damage;
        originalRange = Range;
        currentFlameProjectile = flameProjeciles[Level - 1];
        base.Start();
    }

    public override void ApplyBuff(BuffField buff)
    {
        //if (!IsBuffed)  // 버프가 처음 적용될 때만 원본 값 저장
        //{
        //    originalDamage = Damage;
        //    originalRange = Range;
        //}
        
        base.ApplyBuff(buff);
        
        // 화염 발사체에도 버프 적용
        if (currentFlameProjectile != null)
        {
            FlameProjectile proj = currentFlameProjectile.GetComponent<FlameProjectile>();
            if (proj != null)
            {
                proj.Damage = Mathf.RoundToInt(originalDamage * buff.damageMultiplier);
            }
        }
    }

    public override void RemoveBuff(BuffField buff)
    {
        base.RemoveBuff(buff);
        
        // 화염 발사체 데미지 원래대로 복구
        if (currentFlameProjectile != null)
        {
            FlameProjectile proj = currentFlameProjectile.GetComponent<FlameProjectile>();
            if (proj != null)
            {
                proj.Damage = originalDamage;
            }
        }

        //// 버프가 모두 제거되면 원본 값으로 복구
        //if (!IsBuffed)
        //{
        //    Damage = originalDamage;
        //    Range = originalRange;
        //}
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            if (CurrentTarget != null)
            {
                currentFlameProjectile.SetActive(true);
                FlameProjectile proj = currentFlameProjectile.GetComponent<FlameProjectile>();
                proj.Damage = this.Damage;  // 현재 타워의 데미지 적용
                proj.IsTargeting = this.IsTargeting;
                proj.Target = this.CurrentTarget;
                yield return new WaitForSeconds(FireRate);
            }
            else
            {
                currentFlameProjectile.SetActive(false);
                yield return null;
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
            originalDamage += 3;  // 원본 데미지도 증가
            Damage += 3;
        }
        else if (Level == 3)
        {
            StopCoroutine(Attack());
            currentFlameProjectile.SetActive(false);
            currentFlameProjectile = flameProjeciles[Level - 1];
            StartCoroutine(Attack());
            originalDamage += 1;  // 원본 데미지도 증가
            Damage += 1;
            originalRange += 2f;  // 원본 범위도 증가
            Range += 2f;
        }
    }

    protected override void Update()
    {
        Detect();
        FollowTarget();
        TooltipPopupCheck();
    }
}
