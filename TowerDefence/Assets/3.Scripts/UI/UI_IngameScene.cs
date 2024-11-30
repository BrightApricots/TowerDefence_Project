using TMPro;
using UnityEngine;

public class UI_IngameScene : MonoBehaviour
{
    private static UI_IngameScene instance;
    public static UI_IngameScene Instance { get { return instance; } }

    public TextMeshProUGUI CurrentHp;
    public TextMeshProUGUI MaxHp;
    public TextMeshProUGUI CurrentMoney;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Update()
    {
        CurrentHp.text = $"{GameManager.Instance.CurrentHp}";
        MaxHp.text = $"/{GameManager.Instance.MaxHp}";
        CurrentMoney.text = $"{GameManager.Instance.CurrentMoney}";
    }
}
