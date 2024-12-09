using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_TitleScene : MonoBehaviour
{
    public Button NewGame;
    public Button Continue;
    public TextMeshProUGUI ContiText;
    public Button Exit;
    public GameObject CheckNewGame;

    private void OnEnable()
    {
        NewGame.onClick.AddListener(LoadNewGame);
        Continue.onClick.AddListener(LoadContinue);
        Exit.onClick.AddListener(LoadExit);
        SoundManager.Instance.Play("Bonfire", SoundManager.Sound.Bgm);
    }

    private void Start()
    {
        if (!GameManager.Instance.IsSaved)
        {
            ContiText.color = Color.gray;
        }
    }

    private void LoadNewGame()
    {
        SoundManager.Instance.Play("Click03", SoundManager.Sound.Effect);
        CheckNewGame.SetActive(true);
    }

    private void LoadContinue()
    {
        if (GameManager.Instance.IsSaved)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }

    private void LoadExit()
    {
        Application.Quit();
    }
}
