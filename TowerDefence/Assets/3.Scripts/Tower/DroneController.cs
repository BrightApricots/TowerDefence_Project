using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    public float heightOffset = 5f;
    public float attackRange = 8f;

    public GameObject projectilePrefab;
    public GameObject projectileEffect;
    public GameObject projectileHitEffect;

    public bool IsBomb;
    public Transform muzzlePosition;
    public float shootCooldown = 2f;
    public int damage = 1;
    public Transform HomeTransform;
    
    private Transform homePosition;
    [SerializeField]private Transform currentTarget;
    private bool isReturning;
    private float defaultHeight;
    private Transform towerPosition;
    private float towerRange;
    private Vector3 currentVelocity;
    private float smoothTime = 0.5f;
    private float moveThreshold = 0.1f;
    private float lastShootTime;

    public bool IsTargeting;

    public void Initialize(Transform home, Transform tower, float range)
    {
        homePosition = home;
        towerPosition = tower;
        towerRange = range;
        defaultHeight = home.position.y + heightOffset;
        transform.position = new Vector3(home.position.x, defaultHeight, home.position.z);
    }

    private void Update()
    {
        if (currentTarget != null && !isReturning)
        {
            float distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);
            float distanceToTower = Vector3.Distance(currentTarget.position, towerPosition.position);

            if (distanceToTower > towerRange)
            {
                currentTarget = null;
                ReturnToHome();
            }
            else
            {
                FollowTarget();
                
                if (distanceToTarget <= attackRange)
                {
                    ShootAtTarget();
                }
            }
        }
        else if (isReturning)
        {
            ReturnToHome();
        }

        AdjustHeight();
        UpdateRotation();
    }

    private void FollowTarget()
    {
        float distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);
        
        if (distanceToTarget > attackRange * 0.8f)
        {
            Vector3 targetPos = currentTarget.position + Vector3.up * heightOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime, moveSpeed);
        }
    }

    private void AdjustHeight()
    {
        float targetHeight = currentTarget != null ? currentTarget.position.y + heightOffset : homePosition.position.y + heightOffset; ;
        Vector3 currentPos = transform.position;
        currentPos.y = Mathf.Lerp(currentPos.y, targetHeight, Time.deltaTime * moveSpeed);
        transform.position = currentPos;
    }

    private void ReturnToHome()
    {
        isReturning = true;
        Vector3 targetPosition = new Vector3(homePosition.position.x, defaultHeight, homePosition.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, moveSpeed);
        
        if (Vector3.Distance(transform.position, targetPosition) < moveThreshold)
        {
            isReturning = false;
            currentVelocity = Vector3.zero;
        }
    }

    private void UpdateRotation()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        isReturning = false;
    }

    public bool HasTarget()
    {
        return currentTarget != null && !isReturning;
    }

    private void ShootAtTarget()
    {
        if (Time.time - lastShootTime >= shootCooldown)
        {
            Projectile projectile = ObjectManager.Instance.Spawn<Projectile>(projectilePrefab, muzzlePosition.transform.position, Quaternion.identity);
            projectile.Damage = this.damage;
            projectile.IsTargeting = this.IsTargeting;
            projectile.IsBomb = this.IsBomb;
            projectile.Target = currentTarget;

            //GameObject projectile = Instantiate(projectilePrefab, muzzlePosition.position, Quaternion.identity);
            //Projectile proj = projectile.GetComponent<Projectile>();
            //proj.Damage = damage;
            //proj.IsTargeting = true;
            //proj.IsBomb = this.IsBomb;
            //proj.Target = currentTarget;

            PooledParticle muzzleEffect = ObjectManager.Instance.Spawn<PooledParticle>(
                        projectileEffect, muzzlePosition.position, muzzlePosition.transform.rotation
               );

            muzzleEffect?.Initialize();
            //  GameObject _MuzzlEffect = Instantiate(projectileEffect, muzzlePosition.position, muzzlePosition.transform.rotation);
            //Destroy(_MuzzlEffect, _MuzzlEffect.GetComponent<ParticleSystem>().main.duration);
            lastShootTime = Time.time;
        }
    }
} 