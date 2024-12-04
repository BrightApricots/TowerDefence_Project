using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TowerTooltip : MonoBehaviour
{
    public string Name = "";
    public string Element = "";
    public string Damage = "";
    public string Range = "";
    public string FireRate = "";

    public string DamageDealt = "";
    public string TotalKilled = "";

    public string UpgradePrice = "";
    public string SellPrice = "";
    public string TargetPriority = "";

    public Image TowerImage;  

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI UpgradeText;
    public TextMeshProUGUI InfoText;
    public TextMeshProUGUI SellPriceText;
    public TextMeshProUGUI TargetPriorityText;

    public Button UpgradeButton;
    public Button SellButton;

    private Tower selectedTower; // 현재 선택된 타워

    private void Start()
    {
        UpdateUI();
    }
    public void SetTower(Tower tower)
    {
        selectedTower = tower;
        Name = tower.Name;
        Element = tower.Element;
        Damage = tower.Damage.ToString();
        Range = tower.Range.ToString();
        FireRate = tower.FireRate.ToString();
        DamageDealt = tower.DamageDealt.ToString();
        TotalKilled = tower.TotalKilled.ToString();
        UpgradePrice = $"{tower.UpgradePrice}";
        SellPrice = $"{tower.SellPrice}";
        TargetPriority = tower.TargetPriority;
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        NameText.text = Name;
        InfoText.text =
            $"Element: {Element}" + System.Environment.NewLine +
            $"Damage: {Damage}" + System.Environment.NewLine +
            $"Range: {Range}" + System.Environment.NewLine +
            $"Fire Rate: {FireRate}" + System.Environment.NewLine +
            $"Damage Dealt: {DamageDealt}" + System.Environment.NewLine +
            $"Total Killed: {TotalKilled}";

        TargetPriorityText.text = $"Target: {TargetPriority}";
        SellPriceText.text = $"Sell       ${SellPrice}";
        
        // 최대 레벨이면 업그레이드 버튼 비활성화
        if (selectedTower.Level >= selectedTower.MaxLevel)
        {
            UpgradeText.text = "Max Level";
            UpgradeButton.interactable = false;
        }
        else
        {
            UpgradeText.text = $"Upgrade   ${UpgradePrice}";
            UpgradeButton.interactable = true;
        }

        // 버튼 이벤트 연결
        UpgradeButton.onClick.RemoveAllListeners();
        SellButton.onClick.RemoveAllListeners();
        
        UpgradeButton.onClick.AddListener(OnUpgradeClick);
        SellButton.onClick.AddListener(OnSellClick);
    }

    private void OnUpgradeClick()
    {
        if (selectedTower != null)
        {
            if (GameManager.Instance.CurrentMoney >= int.Parse(UpgradePrice))
            {
                GameManager.Instance.CurrentMoney -= int.Parse(UpgradePrice);
                selectedTower.Upgrade();
                SetTower(selectedTower);
            }
            else
            {
                Debug.Log("Not enough gold!");
            }
        }
    }

    private void OnSellClick()
    {
        if (selectedTower != null)
        {
            // 판매 금액 추가
            GameManager.Instance.CurrentMoney += int.Parse(SellPrice);
            
            // 타워 제거
            GameManager.Instance.PlacedTowerList.Remove(selectedTower);
            Destroy(selectedTower.gameObject);
            
            // 툴팁 제거
            Destroy(gameObject);
        }
    }
}
