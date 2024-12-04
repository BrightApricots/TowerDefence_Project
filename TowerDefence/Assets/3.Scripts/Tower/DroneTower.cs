using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTower : Tower
{
    public DroneController drone;

    public Transform droneHomePosition;
    
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
    }

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
    }

    protected override IEnumerator Attack()
    {
        // 드론이 자체적으로 공격하므로 여기서는 아무것도 하지 않음
        yield break;
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        
        // 드론 성능 향상
        if (drone != null)
        {
            drone.damage = this.Damage;
            drone.attackRange = this.Range * 0.7f;  // 드론의 공격범위는 타워 범위의 70%
            drone.shootCooldown = this.FireRate;
            
            // 레벨에 따른 추가 효과
            switch (Level)
            {
                case 2:
                    drone.moveSpeed *= 1.2f;  // 이동속도 20% 증가
                    break;
                case 3:
                    drone.projectilePrefab = Resources.Load<Transform>("Prefabs/AdvancedDroneProjectile");  // 강화된 투사체로 변경
                    break;
            }
        }
    }
} 