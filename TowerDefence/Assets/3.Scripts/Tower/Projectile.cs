using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public float MoveSpeed = 50f;
    public int Damage = 5;
    public float Duration = 3f;
    public bool IsTargeting = false;
    public bool IsBomb = false;
    public bool isSelfDestroy = false;

    public float BombRange = 3f;
    public Transform Target;
    public GameObject ExplosionParticle;

    private void Start()
    {
        if(isSelfDestroy)
        {
            StartCoroutine(SelfDestroy(Duration));
        }
    }

    
    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (IsTargeting)
        {
            TargettingMove();
        }
        else
        {
            NonTagettingMove();
        }
    }

    protected virtual void TargettingMove()
    {
        if(Target==null)
        {
            Destroy(gameObject);
        }
        Vector3 dir = Target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }

    protected virtual void NonTagettingMove()
    {
        transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsBomb)
        {
            Bomb(other);
        }
        else
        {
            NonBomb(other);
        }
    }

    protected virtual void Bomb(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, BombRange);
            
            Instantiate(ExplosionParticle,transform.position, Quaternion.identity);
            foreach (Collider h in hit)
            {
                if (h.CompareTag("Monster"))
                {
                    other.gameObject.GetComponent<Monster>().TakeDamage(Damage);
                }
            }
            Destroy(gameObject);
        }
    }

    protected virtual void NonBomb(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            if(ExplosionParticle != null)
            {
                GameObject go = Instantiate(ExplosionParticle, transform.position, Quaternion.identity);
                Destroy(go, go.GetComponent<ParticleSystem>().main.duration);
            }
            
            other.gameObject.GetComponent<Monster>().TakeDamage(Damage);
            Destroy(gameObject);
        }

    }

    protected IEnumerator SelfDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, BombRange);
    }
}