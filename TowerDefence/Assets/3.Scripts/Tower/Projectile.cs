using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float MoveSpeed = 50f;
    public int Damage = 5;
    public float Duration = 3f;
    public bool IsTargeting = false;
    public bool IsBomb = false;
    public float BombRange = 0f;
    public Monster Target;

    protected virtual void Update()
    {
        Move();
    }

    protected void Move()
    {
        if(IsTargeting)
        {
            TagettingMove();
        }

        else
        {
            NonTagettingMove(); 
        }
    }

    protected virtual void TagettingMove()
    {
        transform.Translate(Target.transform.position*MoveSpeed*Time.deltaTime);
    }

    protected virtual void NonTagettingMove()
    {
        transform.Translate(Vector3.forward * MoveSpeed*Time.deltaTime);
    }

    protected void OnTriggerEnter(Collider other)
    {
        print("trigger");
        if (IsBomb)
        {
            Bomb(other);
        }

        else
        {
            NonBomb(other);
        }
    }

    private void Bomb(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, BombRange);
            foreach (Collider h in hit)
            {
                if (h.CompareTag("Enemy"))
                {
                    other.gameObject.GetComponent<Monster>().TakeDamage(Damage);
                }
            }
            Destroy(gameObject);
        }
    }

    private void NonBomb(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Monster>().TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
