using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : NetworkBehaviour
{
    NetworkManagerCustom networkManager;
    public TMP_Text winnerName;
    [SyncVar(hook = nameof(OnWinnerNameChanged))]
    public string winnerNameText;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManagerCustom>();
        if (PlayerPrefs.HasKey("WinnerName"))
        {
            winnerNameText = PlayerPrefs.GetString("WinnerName");
        }
        else
            winnerNameText = networkManager.winnerName;
    }

    void OnWinnerNameChanged(string oldValue, string newValue)
    {
        winnerName.text = newValue + " win!";
    }

    public void MainMenuButton()
    {
        if (PlayerPrefs.HasKey("WinnerName"))
        {
            PlayerPrefs.DeleteKey("WinnerName");
        }
        NetworkManager.singleton.StopHost();
        SceneManager.LoadScene("MainMenu");
    }
}
