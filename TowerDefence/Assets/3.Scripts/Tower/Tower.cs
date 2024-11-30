using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour
{
    public int Cost;
    public string Name;
    public int Damage;
    public float ShootCooltime=1f;
    public float ShootRange;
    public string Info;
    public bool IsTargeting;
    public bool IsBomb;
    public Transform TowerHead;
    public Transform Barrel;
    public Transform TowerMuzzle;
    public Transform Projectile;
    public GameObject TowerTooltip;
    public GameObject Quad;
    public Canvas mainCanvas;
    private Transform CurrentTarget=null;
    private GameObject currentTooltip;

    private Vector3 clickmousePointer;


    private void Start()
    {
        GameManager.Instance.PlacedTowerList.Add(this);
        mainCanvas = UI_IngameScene.Instance.GetComponent<Canvas>();
        StartCoroutine(Attack());
    }

    private void Update()
    {
        Detect();
        FollowTarget();
        TooltipPopupCheck();
    }

    protected void Detect()
    {
        if (CurrentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, ShootRange);
            foreach (Collider target in hitColliders)
            {
                if (target.CompareTag("Monster"))
                {
                    CurrentTarget =target.GetComponent<Transform>();
                    break;
                }
            }
        }
        else
        {
            if (Vector3.Distance(CurrentTarget.transform.position, transform.position)>ShootRange)
            {
                CurrentTarget=null;
            }
        }
    }

    protected void FollowTarget()
    {
        if(CurrentTarget !=null)
        {
            Vector3 towerDir = CurrentTarget.transform.position - TowerHead.transform.position;
            if(!IsTargeting)Barrel.forward = towerDir;
            towerDir.y = 0;
            TowerHead.forward = towerDir;
        }
    }

    private void TooltipPopupCheck()
    {
        if (currentTooltip != null && Input.GetMouseButtonDown(0)) //툴팁이 있고, 마우스를 눌렀을 때
        {
            // 마우스 포인터가 UI 위에 없거나, 처음 클릭했던 마우스 위치와 누른 마우스 포지션이 같지 않으면
            if (!EventSystem.current.IsPointerOverGameObject() && clickmousePointer != Input.mousePosition)
            { 
                Destroy(currentTooltip);
                currentTooltip = null;
            }
            //EventSystem.current.IsPointerOverGameObject()
            //UI 요소와의 상호작용을 감지
            //터치와 마우스 입력 모두 지원
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootCooltime);
            if (CurrentTarget !=null)
            {
                Transform projectile = Instantiate(Projectile, TowerMuzzle.transform.position,Barrel.transform.rotation);
                projectile.gameObject.GetComponent<Projectile>().Damage = this.Damage;
                projectile.gameObject.GetComponent<Projectile>().IsTargeting = this.IsTargeting;
                projectile.gameObject.GetComponent<Projectile>().IsBomb = this.IsBomb;
                projectile.gameObject.GetComponent<Projectile>().Target = this.CurrentTarget;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ShootRange);
    }

    private void OnMouseDown() //gameObject 클릭 시 발생 이벤트 MonoBehavior 내장
    {
        clickmousePointer = Input.mousePosition;
        if (currentTooltip != null) //현재 툴팁이 있다면
        {
            Destroy(currentTooltip); //툴팁 제거
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 화면에 Ray 쏘기
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit) )
        {

            //월드 좌표를 스크린 좌표로 변환
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(hit.point);

            //UI 생성 및 위치 설정
            currentTooltip = Instantiate(TowerTooltip, mainCanvas.transform);
            RectTransform rect = currentTooltip.GetComponent<RectTransform>();

            //스크린 좌표를 캔버스 좌표로 변환
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform.GetComponent<RectTransform>(), screenPoint, mainCanvas.worldCamera, out localPoint);

            rect.anchoredPosition = localPoint;
        }
    }
}