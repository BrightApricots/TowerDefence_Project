using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI Upgrade;
    public TextMeshProUGUI InfoText;
    public TextMeshProUGUI SellPriceText;
    public TextMeshProUGUI TargetPriorityText;

    public void Start()
    {
        NameText.text = Name;
        InfoText.text = $"Element: {Element}" + System.Environment.NewLine +
            $"Damage: {Damage}"+ System.Environment.NewLine + 
            $"Range: {Range}" + System.Environment.NewLine +
            $"Fire Rate: {FireRate}" + System.Environment.NewLine + 
            System.Environment.NewLine + System.Environment.NewLine +
            $"Damage Dealt: {DamageDealt}" + System.Environment.NewLine +
            $"Total Killed: {TotalKilled}";
        SellPriceText.text = $"SELL {SellPrice}";
        TargetPriorityText.text = $"Target: {TargetPriority}";
    }

}
