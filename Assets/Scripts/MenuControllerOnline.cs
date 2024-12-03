using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControllerOnline : MonoBehaviourPunCallbacks
{
    public TMP_InputField lobbyCodeInput;
    public Button createLobbyButton;
    public Button joinLobbyButton;
    public Button lobbyListButton;

    private bool isConnecting = false;
    private string roomNameToJoin = "";

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        createLobbyButton.onClick.AddListener(CreateLobby);
        joinLobbyButton.onClick.AddListener(JoinLobby);
        lobbyListButton.onClick.AddListener(ShowLobbyList);
    }

    public void CreateLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            isConnecting = true;
        } else
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
            string roomName = "Room_" + Random.Range(1000, 9999);
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void JoinLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            roomNameToJoin = lobbyCodeInput.text;
            PhotonNetwork.JoinRoom(roomNameToJoin);
        }
        else
        {
            isConnecting = true;
        }
    }

    public void ShowLobbyList()
    {
        // Реализация отображения списка лобби
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            if (roomNameToJoin != "")
            {
                PhotonNetwork.JoinRoom(roomNameToJoin);
            }
            else
            {
                RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
                string roomName = "Room_" + Random.Range(1000, 9999);
                PhotonNetwork.CreateRoom(roomName, roomOptions);
            }
            isConnecting = false;
        }
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to create room: " + message);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
