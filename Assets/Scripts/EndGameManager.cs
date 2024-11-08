using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;
    public TMP_Text winnerNameText;
    public string winnerName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.gameObject.SetActive(true);
            SetWinnerText(GameData.Instance.GetWinnerName());
        }
    }

    
    public void SetWinnerText(string winnerText)
    {
        winnerNameText.text = winnerText+ " win!";
    }

    void OnWinnerNameChanged(string oldValue, string newValue)
    {
        winnerNameText.text = newValue + " win!";
    }

    public void MainMenuButton()
    {
        NetworkManager.singleton.StopHost();
        SceneManager.LoadScene("MainMenu");
    }
}
