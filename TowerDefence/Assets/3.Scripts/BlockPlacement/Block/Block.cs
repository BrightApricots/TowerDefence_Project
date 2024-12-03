using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    protected Button button;
    protected int blockType;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }

    protected virtual void Start()
    {
        button.onClick.AddListener(() => PlacementSystem.Instance.StartPlacement(blockType));
    }
}
