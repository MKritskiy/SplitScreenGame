using Mirror;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviourPunCallbacks
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
        SetWinnerText(PhotonNetwork.CurrentRoom.CustomProperties["Winner"] as string);
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
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
}
