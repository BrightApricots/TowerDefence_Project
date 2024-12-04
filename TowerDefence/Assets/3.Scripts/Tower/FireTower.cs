using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTower : Tower
{
    [SerializeField] private GameObject fireProjectileObject;
    private FireProjectile fireProjectile;

    private void Awake()
    {
        base.Start();
        //fireProjectile = fireProjectileObject.GetComponent<FireProjectile>();
    }

    protected override void Start()
    {
        GameManager.Instance.PlacedTowerList.Add(this);
        mainCanvas = UI_IngameScene.Instance.GetComponent<Canvas>();
        StartCoroutine(Attack());
    }

    protected override void Update()
    {
        Detect();
        FollowTarget();
        TooltipPopupCheck();
    }
    
    protected override IEnumerator Attack()
    {
        while (true)
        {
            if (CurrentTarget != null)
            {
                fireProjectileObject.SetActive(true);
                fireProjectile.Damage = this.Damage;
                fireProjectile.IsTargeting = this.IsTargeting;
                fireProjectile.IsBomb = this.IsBomb;
                fireProjectile.Target = this.CurrentTarget;
                
                //Collider[] hit = Physics.OverlapBox(transform.position - new Vector3(-3, -1, 0), new Vector3(5, 2, 1));
            }
            else
            {
                fireProjectileObject.SetActive(false);
            }
            //yield return new WaitForSeconds(ShootCooltime);
            yield return new WaitForSeconds(1f);
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(transform.position - new Vector3(-3, -1, 0), new Vector3(5, 2, 1));
    //}
}
