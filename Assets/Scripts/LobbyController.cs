using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (PhotonNetwork.CurrentRoom != null)
        {
            playerCountText.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
            startGameButton.interactable = allPlayersReady;
        }
    }
    
    public void OnReadyToggleChanged(bool isReady)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isReady", isReady } });
    }
    
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "PlayerCount", 0 } });
        int playerNum = 1;
        //string playerName = "Player " + playerNum;
        //while (PhotonNetwork.PlayerList.Select(p => p.NickName).Contains(playerName))
        //{
        //    playerName = "Player " + playerNum++;
        //}
        //foreach (Player player in PhotonNetwork.PlayerList)
        //{
        //    player.NickName = "Player " + playerNum++;
        //}
        //PhotonNetwork.LocalPlayer.NickName = playerName;
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
