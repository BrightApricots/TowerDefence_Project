using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_IngameScene : MonoBehaviour
{
    private static UI_IngameScene instance;
    public static UI_IngameScene Instance { get { return instance; } }

    public TextMeshProUGUI CurrentHp;
    public TextMeshProUGUI MaxHp;
    public TextMeshProUGUI CurrentMoney;
    public Transform TowerCardLocation1;
    public Transform TowerCardLocation2;
    public GameObject EmptyCard;
    public GameObject PausePanel; 
    public static int PopupCount;

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

    private void Start()
    {
        TowerCardSet();
    }

    private void Update()
    {
        CurrentHp.text = $"{GameManager.Instance.CurrentHp}";
        MaxHp.text = $"/{GameManager.Instance.MaxHp}";
        CurrentMoney.text = $"{GameManager.Instance.CurrentMoney}";
        if(Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
        }
    }

    private void TowerCardSet()
    {
        for(int i= 0; i<8;i++)
        {
            print(GameManager.Instance.EquipTowerList[i]);
            if (GameManager.Instance.EquipTowerList[i]!="" && GameManager.Instance.EquipTowerList[i]!=null)
            {
                if (i < 4)
                {
                    Instantiate(Resources.Load($"InGameTowerCard/{GameManager.Instance.EquipTowerList[i]}"), TowerCardLocation1);
                }
                else
                {
                    Instantiate(Resources.Load($"InGameTowerCard/{GameManager.Instance.EquipTowerList[i]}"), TowerCardLocation2);
                }
            }
            else
            {
                if (i< 4)
                {
                    Instantiate(EmptyCard, TowerCardLocation1);
                }
                else
                {
                    Instantiate(EmptyCard, TowerCardLocation2);
                }
            }
        }
    }
}
