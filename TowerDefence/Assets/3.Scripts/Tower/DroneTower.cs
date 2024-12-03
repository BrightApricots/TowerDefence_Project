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
            drone.Initialize(droneHomePosition, transform, ShootRange);
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
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, ShootRange);
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
} 