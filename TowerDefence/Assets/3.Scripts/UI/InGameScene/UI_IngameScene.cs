using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_IngameScene : MonoBehaviour
{
    // 싱글턴 인스턴스
    private static UI_IngameScene instance;
    public static UI_IngameScene Instance { get { return instance; } }

    // UI 요소 정의
    public TextMeshProUGUI CurrentHp; // 현재 HP를 표시하는 텍스트
    public TextMeshProUGUI MaxHp; // 최대 HP를 표시하는 텍스트
    public TextMeshProUGUI CurrentMoney; // 현재 소지 금액을 표시하는 텍스트
    public Transform TowerCardLocation1; // 상단 타워 카드 위치
    public Transform TowerCardLocation2; // 하단 타워 카드 위치
    public GameObject EmptyCard; // 빈 카드 오브젝트
    public GameObject PausePanel; // 일시정지 패널
    public static int PopupCount; // 팝업 카운트(사용 여부에 따라 확장 가능)

    private void Awake()
    {
        // 싱글턴 초기화
        if(instance == null)
        {
            instance = this; // 현재 인스턴스 설정
        }
        else
        {
            DestroyImmediate(this); // 중복된 인스턴스 제거
        }
    }

    private void Start()
    {
        // 배경 음악 재생
        SoundManager.Instance.Play("Battlefield", SoundManager.Sound.Bgm);

        // 타워 카드 설정
        TowerCardSet();
    }

    private void Update()
    {
        // 실시간 UI 업데이트
        CurrentHp.text = $"{GameManager.Instance.CurrentHp}"; // 현재 HP 갱신
        MaxHp.text = $"/{GameManager.Instance.MaxHp}"; // 최대 HP 갱신
        CurrentMoney.text = $"{GameManager.Instance.CurrentMoney}"; // 현재 금액 갱신

        // 'Cancel' 키 입력 시 일시정지
        if(Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 0f; // 게임 정지
            PausePanel.SetActive(true); // 일시정지 패널 활성화
        }
    }

    private void TowerCardSet()
    {
        // 총 8개의 타워 카드 설정
        for(int i = 0; i < 8; i++)
        {
            // 현재 장착된 타워 출력 (디버그용)
            print(GameManager.Instance.EquipTowerList[i]);

            // 장착된 타워가 있는 경우
            if (GameManager.Instance.EquipTowerList[i] != "" && GameManager.Instance.EquipTowerList[i] != null)
            {
                if (i < 4) // 상단 타워 카드 위치
                {
                    Instantiate(Resources.Load($"InGameTowerCard/{GameManager.Instance.EquipTowerList[i]}"), TowerCardLocation1);
                }
                else // 하단 타워 카드 위치
                {
                    Instantiate(Resources.Load($"InGameTowerCard/{GameManager.Instance.EquipTowerList[i]}"), TowerCardLocation2);
                }
            }
            // 장착된 타워가 없는 경우 빈 카드 생성
            else
            {
                if (i < 4) // 상단 빈 카드 위치
                {
                    Instantiate(EmptyCard, TowerCardLocation1);
                }
                else // 하단 빈 카드 위치
                {
                    Instantiate(EmptyCard, TowerCardLocation2);
                }
            }
        }
    }
}
