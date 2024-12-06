using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform OriginalParent;
    public Vector3 OriginalPosition;

    public void SetPosition(Vector3 position)
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.position = position;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OriginalParent = transform.parent;
        OriginalPosition = transform.position;
        transform.SetParent(transform.root);
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    GetComponent<Image>().raycastTarget = true;
    //    transform.SetParent(OriginalParent);
    //    transform.position = OriginalPosition;
    //}

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End drag: {gameObject.name}");

        GetComponent<Image>().raycastTarget = true;

        // 유효한 슬롯 위에 드롭되지 않은 경우
        if (eventData.pointerEnter == null ||
            !eventData.pointerEnter.GetComponent<UI_TowerLoadoutSlot>())
        {
            Debug.Log("Returning to original position");
            transform.SetParent(OriginalParent);
            transform.position = OriginalPosition;
        }
    }
}
