using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public NetworkManager networkManager;
    public TMP_Text playerCountText;
    public Button startGameButton;
    private int playerCount = 0;
    void Start()
    {
        if (PlayerPrefs.HasKey("HostAddress"))
        {
            string hostAddress = PlayerPrefs.GetString("HostAddress");
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
        }
        else
        {
            networkManager.StartHost();
        }

        networkManager.playerPrefab = Resources.Load<GameObject>("Player"); // Убедитесь, что префаб игрока находится в папке Resources
    }

    void Update()
    {
        if (networkManager.isNetworkActive)
        {
            playerCountText.text = "Players: " + playerCount;
            //startGameButton.interactable = networkManager.numPlayers > 1;
        }
    }

    public void StartGame()
    {
        
        networkManager.ServerChangeScene("GameSceneOnline");
    }

    public void ExitLobby()
    {
        networkManager.StopHost();
        networkManager.StopClient();
        SceneManager.LoadScene("MainMenu");
    }
    private void ResetPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("HostAddress"))
        {
            PlayerPrefs.DeleteKey("HostAddress");
        }
    }
}
