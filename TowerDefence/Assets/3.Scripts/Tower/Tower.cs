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
    //public float Range;
    public float FireRate;
    public int DamageDealt;
    public int TotalKilled;
    public int UpgradePrice;
    public int SellPrice;
    public string TargetPriority;
    public string Info;
    #endregion

    public GameObject rangeIndicatorPrefab;  // 라인 렌더러를 가진 프리팹
    private RangeIndicator rangeIndicator;

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

    [SerializeField]
    private float _range;  // private 필드 추가
    public float Range  // 프로퍼티로 변경
    {
        get { return _range; }
        protected set
        {
            _range = value;
            // Range가 변경될 때마다 RangeIndicator 업데이트
            if (rangeIndicator != null)
            {
                rangeIndicator.UpdateRange(_range);
            }
        }
    }

    protected virtual void Start()
    {
        // 라인 렌더러 오브젝트를 자식으로 생성
        GameObject indicatorObj = Instantiate(rangeIndicatorPrefab, transform);
        rangeIndicator = indicatorObj.GetComponent<RangeIndicator>();
        rangeIndicator.Initialize(Range);  // 초기 Range 설정
        rangeIndicator.gameObject.SetActive(true);
        if (!isPreview)
        {
            GameManager.Instance.PlacedTowerList.Add(this);
            mainCanvas = UI_IngameScene.Instance.GetComponent<Canvas>();
            StartCoroutine(Attack());
            rangeIndicator.gameObject.SetActive(false);  // preview가 아닐 때만 숨김
        }
    }

    protected virtual void Update()
    {
        if (!isPreview)
        {
            Detect();
        }
        if(isFollow)
        {
            FollowTarget();
        }
        TooltipPopupCheck();

        // preview 상태일 때 indicator 위치 업데이트
        if (!isPreview && rangeIndicator != null)
        {
            rangeIndicator.UpdatePosition();
        }
    }

    protected virtual void OnLevelUp()
    {
        // 각 타워에서 Range 값을 변경하면 자동으로 RangeIndicator가 업데이트됨
        // Range = newRangeValue;  // 각 타워 클래스에서 이렇게 설정
    }

    protected virtual void Detect()
    {
        if (CurrentTarget == null || !CurrentTarget.gameObject.activeSelf)
        {
            float closestDistance = float.MaxValue;
            Transform closestTarget = null;
            
            // Range 프로퍼티 사용
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
            foreach (Collider target in hitColliders)
            {
                if (target != null && target.CompareTag("Monster"))
                {
                    Monster monster = target.GetComponent<Monster>();
                    if (monster != null && monster.gameObject.activeSelf)  // 몬스터가 유효하고 활성화 상태인지 확인
                    {
                        float distance = Vector3.Distance(target.transform.position, transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = target.transform;
                        }
                    }
                }
            }
            CurrentTarget = closestTarget;
        }
        else
        {
            // 현재 타겟이 범위를 벗어났는지 확인할 때도 Range 프로퍼티 사용
            if (Vector3.Distance(CurrentTarget.transform.position, transform.position) > Range || 
                !CurrentTarget.gameObject.activeSelf)
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
                if (rangeIndicator != null)
                {
                    rangeIndicator.gameObject.SetActive(false);
                }
                Destroy(currentTooltip);
                GameManager.Instance.tooltipCount = false;
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
        // Range 프로퍼티 사용
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
            if (rangeIndicator != null)
            {
                rangeIndicator.gameObject.SetActive(true);
            }

            clickmousePointer = Input.mousePosition;
            if (currentTooltip != null)
            {
                GameManager.Instance.tooltipCount = false;
                Destroy(currentTooltip);

                if (rangeIndicator != null)
                {
                    rangeIndicator.gameObject.SetActive(false);
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(GameManager.Instance.tooltipCount != true)
            {
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
                    GameManager.Instance.tooltipCount = true;
                }
            }
            
        }
    }
}