using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed=4f;
    public float damage;
    public float duration=3f;

    private void Update()
    {
        Move(Vector3.forward);
    }

    private void Move(Vector3 dir)
    {
        transform.Translate(dir*moveSpeed*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            
        }
    }
}
