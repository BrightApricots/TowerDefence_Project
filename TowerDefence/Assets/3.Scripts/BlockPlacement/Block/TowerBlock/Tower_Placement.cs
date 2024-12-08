using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Placement : Block
{
    [SerializeField]
    protected int towerID = 101;  // 기본값 설정, 자식 클래스에서 접근 가능하도록 protected로 변경
    public int TowerPrice;

    protected override void Awake()
    {
        blockType = towerID;  // towerID 사용
        base.Awake();
    }

    protected override void Start()
    {
        button.onClick.AddListener(PlaceTower);  // Placement 대신 PlaceTower 사용
        // 타워는 재사용 가능하므로 OnPlacementSuccess 이벤트를 구독하지 않음
    }

    private void PlaceTower()
    {
        if (TowerPrice <= GameManager.Instance.CurrentMoney)
        {
            PlacementSystem.Instance.StartTowerPlacement(blockType);  // blockType 사용
        }
        else
        {
            Debug.Log("Not enough money to place tower");
        }
    }

    // 타워는 설치 후에도 제거되지 않도록 HandlePlacementSuccess를 오버라이드
    protected override void HandlePlacementSuccess(int placedBlockID, string selectedCardID)
    {
        // 아무 동작도 하지 않음 (카드가 제거되지 않음)
    }
}
