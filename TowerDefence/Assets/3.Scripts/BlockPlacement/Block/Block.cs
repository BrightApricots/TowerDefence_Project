using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    protected Button button;
    protected int blockType;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }   

    protected virtual void Start()
    {
        button.onClick.AddListener(Placement);
    }

    protected virtual void Placement()
    {
        PlacementSystem.Instance.StartPlacement(blockType);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        print("ondrag");
        PlacementSystem.Instance.StartPlacementForDrag(blockType);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PlacementSystem.Instance.StopPlacementForDrag();
    }
}
