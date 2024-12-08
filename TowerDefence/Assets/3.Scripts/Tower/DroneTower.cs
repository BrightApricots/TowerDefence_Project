using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTower : Tower
{
    public DroneController drone;
    public Transform droneHomePosition;

    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private List<GameObject> muzzleEffects;
    [SerializeField] private List<GameObject> HitEffects;

    public DroneTower()
    {
        Name = "Drone Tower";
        Element = "Electric";
        Damage = 2;
        Range = 5f;
        FireRate = 2f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 15;
        SellPrice = 8;
        TargetPriority = "Most Progress";
        Info = "Automatically tracks and continuously attacks enemies within its range until they leave.";
    }

    protected override void Start()
    {
        base.Start();

        if (drone != null && droneHomePosition != null)
        {
            drone.Initialize(droneHomePosition, transform, Range);
        }
        else
        {
            Debug.LogError("드론 또는 홈 포지션을 찾을 수 없습니다!");
        }
        drone.projectilePrefab = projectiles[Level - 1];
        drone.projectileEffect = muzzleEffects[Level - 1];
        drone.projectileHitEffect = HitEffects[Level - 1];
    }

    //protected virtual void Detect()
    //{
    //    if (CurrentTarget == null)
    //    {
    //        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
    //        foreach (Collider target in hitColliders)
    //        {
    //            if (target.CompareTag("Monster"))
    //            {
    //                CurrentTarget = target.GetComponent<Transform>();
    //                break;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (Vector3.Distance(CurrentTarget.transform.position, transform.position) > Range)
    //        {
    //            CurrentTarget = null;
    //        }
    //    }
    //}

    protected override void Detect()
    {
        if (!drone.HasTarget())
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
            Transform nearestTarget = null;
            float minDistance = float.MaxValue;

            foreach (Collider target in hitColliders)
            {
                if (target.CompareTag("Monster"))
                {
                    float distance = Vector3.Distance(drone.transform.position, target.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestTarget = target.transform;
                    }
                }
            }

            if (nearestTarget != null)
            {
                drone.SetTarget(nearestTarget);
            }
        }
        else
        {
            if (Vector3.Distance(CurrentTarget.transform.position, transform.position) > Range)
            {
                CurrentTarget = null;
            }
        }
    }

    protected override IEnumerator Attack()
    {
        // 드론이 자체적으로 공격하므로 여기서는 아무것도 하지 않음
        yield break;
    }

    protected override void OnLevelUp()
    {
        if (Level == 2)
        {
            drone.moveSpeed *= 1.3f;
            drone.attackRange += 1.5f;
            drone.shootCooldown *= 0.5f;
            SetByLevel();
        }
        else if (Level == 3)
        {
            drone.damage += 1;
            drone.shootCooldown *= 0.2f;
            SetByLevel();
        }
    }

    private void SetByLevel()
    {
        drone.projectilePrefab = projectiles[Level - 1];
        drone.projectileEffect = muzzleEffects[Level - 1];
        drone.projectileHitEffect = HitEffects[Level - 1];
    }
} 