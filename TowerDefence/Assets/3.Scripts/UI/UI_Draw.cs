using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Draw : MonoBehaviour
{
    public int DrawPrice = 5;
    public GameObject OneSquarePrefab;
    public GameObject FourSquarePrefab;
    public GameObject FuckYouPrefab;
    public GameObject ZigzagPrefab;
    public GameObject StraightPrefab;
    public GameObject LPrefab;
    public Button DrawButton;
    public TextMeshProUGUI DrawPriceText;
    public RectTransform HorizontalLayout;

    private void Awake()
    {
        DrawButton.onClick.AddListener(DrawCard);
    }

    private void Update()
    {
        DrawPriceText.text = $"{DrawPrice}";
    }

    public void DrawCard()
    {
        if(GameManager.Instance.HandTetrisList.Count < 7 && GameManager.Instance.CurrentMoney >= DrawPrice)
        { 
            GameManager.Instance.CurrentMoney -= DrawPrice;
            int drawCard = Random.Range(0, GameManager.Instance.TotalTetrisList.Count);
            for (int i = 0; i < GameManager.Instance.TotalTetrisList.Count; i++)
            {
                if (i == drawCard)
                {
                    GameObject type;
                    switch (GameManager.Instance.TotalTetrisList[i])
                    {
                        case 0:
                            type = Instantiate(OneSquarePrefab, HorizontalLayout);
                            GameManager.Instance.HandTetrisList.Add(type);
                            break;
                        case 1:
                            type = Instantiate(FourSquarePrefab, HorizontalLayout);
                            GameManager.Instance.HandTetrisList.Add(type);
                            break;
                        case 2:
                            type = Instantiate(FuckYouPrefab, HorizontalLayout);
                            GameManager.Instance.HandTetrisList.Add(type);
                            break;
                        case 3:
                            type = Instantiate(ZigzagPrefab, HorizontalLayout);
                            GameManager.Instance.HandTetrisList.Add(type);
                            break;
                        case 4:
                            type = Instantiate(StraightPrefab, HorizontalLayout);
                            GameManager.Instance.HandTetrisList.Add(type);
                            break;
                        case 5:
                            type = Instantiate(LPrefab, HorizontalLayout);
                            GameManager.Instance.HandTetrisList.Add(type);
                            break;
                    }
                    DrawPrice += 5;
                    break;
                }
            }
        }
    }
}
