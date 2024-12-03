using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public TMP_Text playerCountText;
    public TMP_Text lobbyCodeText;
    public Button startGameButton;
    public Toggle readyToggle;

    private bool allPlayersReady = false;

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            startGameButton.gameObject.SetActive(false);

        lobbyCodeText.text = "Lobby Code: " + PhotonNetwork.CurrentRoom.Name;
        startGameButton.interactable = false;

        readyToggle.onValueChanged.AddListener(OnReadyToggleChanged);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        playerCountText.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        startGameButton.interactable = allPlayersReady;
    }
    
    public void OnReadyToggleChanged(bool isReady)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isReady", isReady } });
    }
    
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameSceneOnline");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isReady"))
        {
            CheckAllPlayersReady();
        }
    }

    private void CheckAllPlayersReady()
    {
        allPlayersReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isReady;
            if (player.CustomProperties.TryGetValue("isReady", out isReady))
            {
                if (!(bool)isReady)
                {
                    allPlayersReady = false;
                    break;
                }
            }
            else
            {
                allPlayersReady = false;
                break;
            }
        }
    }

    public void ExitBtn()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
