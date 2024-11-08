using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public NetworkManagerCustom networkManager;
    //public ClientCounter clientCounter;
    public TMP_Text playerCountText;
    public TMP_Text addressText;

    public Button startGameButton;
    void Start()
    {
        if (NetworkManager.singleton == null)
        {
            GameObject networkManagerObject = new GameObject("NetworkManager");
            networkManager = networkManagerObject.AddComponent<NetworkManagerCustom>();
            DontDestroyOnLoad(networkManagerObject);
        }
        else
        {
            networkManager = NetworkManager.singleton as NetworkManagerCustom;
        }
        networkManager.ResetPlayerCount();
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
        addressText.text = "Address: " + networkManager.networkAddress;
        //clientCounter = networkManager.clientCounter;
    }

    void Update()
    {
        
        if (networkManager.isNetworkActive)
        {
            //playerCountText.text = "Players: " + clientCounter.clientsCount;
            //startGameButton.interactable = networkManager.numPlayers > 1;
        }
    }

    public void StartGame()
    {
        networkManager.StartGame();
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
