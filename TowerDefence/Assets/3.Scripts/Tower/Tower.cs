using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    #region ToolTip
    [Header("타워 정보")]
    public string Name;
    public string Element;
    public int Damage;
    public float Range;
    public float FireRate;
    public int DamageDealt;
    public int TotalKilled;
    public int UpgradePrice;
    public int SellPrice;
    public string TargetPriority;
    public string Info;
    #endregion

    public int Level { get; protected set; } = 1;
    public int MaxLevel = 3;

    [Header("타워 파츠")]
    public Transform TowerHead;
    public Transform TowerMuzzle;
    public GameObject TowerTooltip;

    [Header("공통 기능")]
    [Tooltip("타워가 프리뷰 상태에서 발사하지 않게함")]
    public bool isPreview = false;
    [Tooltip("타워가 타겟을 바라봄")]
    public bool isFollow = false;

    [Header("툴팁 캔버스")]
    public Canvas mainCanvas;

    protected Transform CurrentTarget = null;
    private GameObject currentTooltip;
    private Vector3 clickmousePointer;

    protected virtual void Start()
    {
        if(!isPreview)
        {
            GameManager.Instance.PlacedTowerList.Add(this);
            mainCanvas = UI_IngameScene.Instance.GetComponent<Canvas>();
            StartCoroutine(Attack());
        }
    }

    protected virtual void Update()
    {
        if (!isPreview)
        {
            Detect();
        }
        if(!isFollow)
        {
            FollowTarget();
        }
        TooltipPopupCheck();
    }

    protected virtual void OnLevelUp()
    {
        // 각 타워에서 업그레이드 구현
    }


    protected virtual void Detect()
    {
        if (CurrentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
            foreach (Collider target in hitColliders)
            {
                if (target.CompareTag("Monster"))
                {
                    CurrentTarget = target.GetComponent<Transform>();
                    break;
                }
            }
        }
        else
        {
            if (Vector3.Distance(CurrentTarget.transform.position, transform.position)> Range)
            {
                CurrentTarget = null;
            }
        }
    }

    protected virtual void FollowTarget()
    {
        if(CurrentTarget != null)
        {
            Vector3 towerDir = CurrentTarget.transform.position - TowerHead.transform.position;
            towerDir.y = 0;
            TowerHead.forward = towerDir;
        }
    }

    protected void TooltipPopupCheck()
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

    protected virtual IEnumerator Attack()
    {
        //각자 타워에서 구현
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }

    public virtual void Upgrade()
    {
        if (Level >= MaxLevel)
        {
            Debug.Log("타워 최대레벨 초과");
            return;
        }

        Level++;
        OnLevelUp();
    }

    private void OnMouseDown()
    {
        if (!isPreview)
        {
                clickmousePointer = Input.mousePosition;
            if (currentTooltip != null)
            {
                Destroy(currentTooltip);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(hit.point);

                currentTooltip = Instantiate(TowerTooltip, mainCanvas.transform);
                UI_TowerTooltip toolTip = currentTooltip.GetComponent<UI_TowerTooltip>();
                toolTip.Name = $"{this.Name}";
                toolTip.Element = $"{this.Element}";
                toolTip.Damage = $"{this.Damage}";
                toolTip.Range = $"{this.Range}";
                toolTip.FireRate = $"{this.FireRate}";
                toolTip.DamageDealt = $"{this.DamageDealt}";
                toolTip.UpgradePrice = $"{this.UpgradePrice}";
                toolTip.SellPrice = $"{this.SellPrice}";
                toolTip.TargetPriority = $"{this.TargetPriority}";
                toolTip.TotalKilled = $"{this.TotalKilled}";
                toolTip.TowerImage.sprite = Resources.Load<Sprite>($"Sprites/{this.Name}");

                toolTip.SetTower(this);

                RectTransform rect = currentTooltip.GetComponent<RectTransform>();

                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    mainCanvas.transform.GetComponent<RectTransform>(),
                    screenPoint, mainCanvas.worldCamera, out localPoint);

                rect.anchoredPosition = localPoint;
            }
        }
    }
}