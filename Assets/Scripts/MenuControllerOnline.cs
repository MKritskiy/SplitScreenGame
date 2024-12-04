using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject lobbyListPanel;
    public GameObject lobbyListItemPrefab;
    public Transform lobbyListContent;
    public GameObject mainMenuPanel;

    private string roomNameToJoin = "";

    void Start()
    {
        createLobbyButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        createLobbyButton.onClick.AddListener(CreateLobby);
        joinLobbyButton.onClick.AddListener(JoinLobby);
        lobbyListButton.onClick.AddListener(ShowLobbyList);
        lobbyListPanel.SetActive(false);
    }

    public void CreateLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
            string roomName = "Room_" + Random.Range(1000, 9999);
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void JoinLobby()
    {
        
        roomNameToJoin = lobbyCodeInput.text;
        PhotonNetwork.JoinRoom(roomNameToJoin);

    }

    public void ShowLobbyList()
    {

        lobbyListPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnectedToMaster()
    {

        if (roomNameToJoin != "")
        {
            PhotonNetwork.JoinRoom(roomNameToJoin);
        }
        createLobbyButton.interactable = true;

    }

    public override void OnJoinedRoom()
    {
        SetUniquePlayerName();
        SceneManager.LoadScene("LobbyScene");
    }
    private void SetUniquePlayerName()
    {
        int playerNum = 1;
        string playerName = "Player " + playerNum;
        while (PhotonNetwork.PlayerList.Select(p => p.NickName).Contains(playerName))
        {
            playerName = "Player " + playerNum++;
        }
        PhotonNetwork.LocalPlayer.NickName = playerName;
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
        PhotonNetwork.AutomaticallySyncScene = false;

        SceneManager.LoadScene("MainMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in lobbyListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            GameObject lobbyItem = Instantiate(lobbyListItemPrefab, lobbyListContent);
            TextMeshProUGUI lobbyNameText = lobbyItem.GetComponentInChildren<TextMeshProUGUI>();
            Button joinButton = lobbyItem.GetComponentInChildren<Button>();

            lobbyNameText.text = room.Name;
            joinButton.onClick.AddListener(() => JoinLobbyByName(room.Name));
        }
    }

    private void JoinLobbyByName(string roomName)
    {
        roomNameToJoin = roomName;
        PhotonNetwork.JoinRoom(roomNameToJoin);
        lobbyListPanel.SetActive(false);
    }
}
