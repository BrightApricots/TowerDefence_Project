//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class UI_TowerLoadoutSlot : MonoBehaviour, IDropHandler
//{
//    public SlotType slotType;
//    private UI_Draggable currentTower;

//    public enum SlotType
//    {
//        Inventory,
//        Equipment
//    }

//    public void OnDrop(PointerEventData eventData)
//    {
//        print("OnDrop");
//        GameObject droppedObject = eventData.pointerDrag;
//        UI_Draggable draggableItem = droppedObject.GetComponent<UI_Draggable>();
//        if (currentTower != null)
//        {
//            //if (transform.childCount > 0)
//            //{
//            //    Transform existingItem = transform.GetChild(0);
//            //    existingItem.SetParent(draggableItem.OriginalParent);
//            //    existingItem.position = draggableItem.OriginalPosition;
//            //}

//            eventData.pointerDrag.transform.SetParent(transform);
//            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
//        }
//    }

//}

using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TowerLoadoutSlot : MonoBehaviour, IDropHandler
{
    public SlotType slotType;
    private UI_Draggable currentTower;
    private RectTransform rectTransform;

    public enum SlotType
    {
        Inventory,
        Equipment
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Image 컴포넌트가 없다면 추가
        if (!GetComponent<UnityEngine.UI.Image>())
        {
            gameObject.AddComponent<UnityEngine.UI.Image>().raycastTarget = true;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"OnDrop called on slot: {gameObject.name}");

        if (eventData.pointerDrag == null)
        {
            Debug.LogWarning("No dragged object found");
            return;
        }

        UI_Draggable draggedTower = eventData.pointerDrag.GetComponent<UI_Draggable>();
        if (draggedTower == null)
        {
            Debug.LogWarning("Dragged object is not a tower");
            return;
        }

        // 현재 슬롯의 타워와 위치 교환
        SwapTowers(draggedTower);
    }

    private void SwapTowers(UI_Draggable draggedTower)
    {
        Transform draggedOriginalParent = draggedTower.OriginalParent;
        Vector3 draggedOriginalPosition = draggedTower.OriginalPosition;

        // 현재 슬롯에 타워가 있는 경우
        if (transform.childCount > 0)
        {
            Transform existingTower = transform.GetChild(0);
            UI_Draggable existingDraggable = existingTower.GetComponent<UI_Draggable>();

            if (existingDraggable != null)
            {
                // 기존 타워를 드래그된 타워의 원래 위치로 이동
                existingTower.SetParent(draggedOriginalParent);
                existingDraggable.SetPosition(draggedOriginalPosition);
            }
        }

        // 드래그된 타워를 현재 슬롯으로 이동
        draggedTower.transform.SetParent(transform);
        draggedTower.SetPosition(rectTransform.position);

        Debug.Log($"Successfully swapped towers in slot {gameObject.name}");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<UnityEngine.UI.Image>() == null)
        {
            Debug.LogWarning($"UI_TowerLoadoutSlot on {gameObject.name} requires an Image component!");
        }
    }
#endif
}
