using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeDisapearEffect : MonoBehaviour
{
    public ParticleSystem effect;
    public float checkRadius = .5f;

    private void Awake()
    {
        effect = Resources.Load<ParticleSystem>("Effects/TreeEffect");

        // 트리거 콜라이더 설정
        SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.radius = checkRadius;
        triggerCollider.isTrigger = true;
    }

    // OnTriggerEnter 제거 - 이제 PlacementState에서 직접 처리
}