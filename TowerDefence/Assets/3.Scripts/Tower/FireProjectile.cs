using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireProjectile : Projectile
{
    public float DamageInterval = 0.5f;
    protected override void Update()
    {

    }

    private Dictionary<Collider, Coroutine> _damageCoroutines = new Dictionary<Collider, Coroutine>();

    //private Coroutine _coStartDamage;
    protected override void OnTriggerEnter(Collider other)
    {
        if (_damageCoroutines.ContainsKey(other))
        {
            StopCoroutine(_damageCoroutines[other]);
        }

        // 새로운 코루틴 시작 및 저장
        var coroutine = StartCoroutine(CoStartDamage(other));
        _damageCoroutines[other] = coroutine;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_damageCoroutines.ContainsKey(other))
        {
            StopCoroutine(_damageCoroutines[other]);
            _damageCoroutines.Remove(other);
        }
    }

    public IEnumerator CoStartDamage(Collider other)
    {
        while (true)
        {
            other.gameObject.GetComponent<Monster>().TakeDamage(this.Damage);
            yield return new WaitForSeconds(DamageInterval);
        }
    }
}
