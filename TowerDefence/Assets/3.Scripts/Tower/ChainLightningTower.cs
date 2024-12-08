using System.Collections;
using System.Drawing;
using UnityEngine;

public class ChainLightningTower : Tower
{
    public int ChainCount = 3;  //체인 횟수
    public float ChainRange = 5f; //다음 타겟 탐지거리

    public GameObject lightningEffectPrefab;

    public ChainLightningTower()
    {
        Name = "Lightning Tower";
        Element = "Electric";
        Damage = 4;
        Range = 4f;
        FireRate = 0.75f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 25;
        SellPrice = 13;
        TargetPriority = "Most Progress";
        Info = "Fires chain lightning that bounces between multiple targets.";
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate);
            if (CurrentTarget != null)
            {
                StartCoroutine(ChainLightning(CurrentTarget, Damage, ChainCount));
            }
        }
    }

    private IEnumerator ChainLightning(Transform target, int damage, int remainingChains)
    {
        if (target == null || remainingChains <= 0)
        {
            yield break;
        }

        Monster monster = target.GetComponent<Monster>();
        if (monster != null)
        {
            monster.TakeDamage(damage);
            
            CreateLightningEffect(TowerMuzzle.position, target.position);

            Transform nextTarget = FindNextTarget(target.position, target);
            if (nextTarget != null && remainingChains > 0)
            {
                yield return new WaitForSeconds(0.1f);
                CreateLightningEffect(target.position, nextTarget.position);
                //체인 후 데미지 70%, 체인카운트 -1
                StartCoroutine(ChainLightning(nextTarget, Mathf.RoundToInt(damage * 0.7f), remainingChains - 1));
            }
        }
    }

    private Transform FindNextTarget(Vector3 position, Transform currentTarget)
    {
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;

        Collider[] hitColliders = Physics.OverlapSphere(position, ChainRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Monster") && collider.transform != currentTarget)
            {
                float distance = Vector3.Distance(position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = collider.transform;
                }
            }
        }
        return closestTarget;
    }

    //지금 맞은 몬스터와 타겟 사이에 라이트닝 생성
    private void CreateLightningEffect(Vector3 start, Vector3 end)
    {
        LightningEffect lightning = ObjectManager.Instance.Spawn<LightningEffect>(lightningEffectPrefab, transform.position);
        lightning.CreateLightning(start, end);
    }

    protected override void OnLevelUp()
    {
        LightningEffect effect = lightningEffectPrefab.GetComponent<LightningEffect>();
        
        switch (Level)
        {
            case 1:
                UnityEngine.Color color = new Color32(240, 255, 83, 255);
                effect.lightningColor = color;
                break;
            case 2:
                color = new Color32(84, 108, 255, 255);
                effect.lightningColor = color;
                ChainCount += 4;
                break;
            case 3:
                Damage += 2;
                FireRate *= 0.5f;
                color = new Color32(255, 83, 243, 255);
                effect.lightningColor = color;
                break;
        }
    }
} 
