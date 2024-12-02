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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Monster")
    //    {
    //        if (_coStartDamage != null)
    //            StopCoroutine(_coStartDamage);

    //        _coStartDamage = StartCoroutine(CoStartDamage(Target.gameObject));
    //    }
    //}

    //public virtual void OnCollisionExit2D(Collision2D collision)
    //{
    //    Monster target = collision.gameObject.GetComponent<Monster>(); // 충돌한 오브젝트(플레이어)를 타켓으로 설정한다

    //    if (collision.gameObject.tag == "Player")
    //    {
    //        if (_coStartDamage != null)
    //        {
    //            StopCoroutine(_coStartDamage);
    //        }
    //        _coStartDamage = null;
    //    }
    //}


    //public IEnumerator CoStartDamage(Monster target)
    //{
    //    while (true)
    //    {
    //        target.OnDamaged(this, 2);
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}


}
