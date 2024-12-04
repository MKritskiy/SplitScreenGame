using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public Vector2 gameFieldSize = new Vector2(10f, 10f);
    public GameObject cameraPrefab;
    public GameObject endGameScreen;
    public TextMeshProUGUI winnerNumber;
    
    public int playerCount = 0;
    private int playerIndex = 0;
    public bool isNetworkGame = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (!isNetworkGame)
        {
            int numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 1);
            playerCount = 0;
            SpawnPlayers(numberOfPlayers);
        } else
        {
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            SpawnPlayers(1);
            
        }
        endGameScreen.SetActive(false);
    }

    public void EndGame(string winnerName)
    {
        if (isNetworkGame)
        {
            PhotonNetwork.CurrentRoom.CustomProperties["Winner"] = winnerName;
            PhotonNetwork.LoadLevel("EndOnlineGameScene");
        }
        else
        {
            endGameScreen.SetActive(true);
            winnerNumber.text = winnerName + " win!";
        }
    }

    public void SpawnPlayers(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Transform spawnPoint;
            GameObject player;
            if (spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
            } 
            else
            {
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-gameFieldSize.x / 2, gameFieldSize.x / 2),
                    0f,
                    Random.Range(-gameFieldSize.y / 2, gameFieldSize.y / 2)
                );
                player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
            }
            if (!isNetworkGame)
            {
                player.GetComponent<PlayerControllerLocal>().playerName = "Player " + (playerCount + 1);
                SetupCamera(numberOfPlayers, player.GetComponent<PlayerControllerLocal>().cameraSpawnPoint);
            } else
            {
                player.GetComponent<PlayerControllerOnline>().playerName = PhotonNetwork.LocalPlayer.NickName;
            }
        }
    }
    public void SetupCamera(int numbersOfPlayers, Transform cameraSpawnPoint)
    {
        Camera camera = cameraSpawnPoint.gameObject.GetComponentInChildren<Camera>();
        Camera UiCamera = camera.GetComponentsInChildren<Camera>()[1];
        UiCamera.enabled = true;
        camera.enabled = true;
        if (isNetworkGame)
        {
            camera.rect = new Rect(0, 0, 1, 1);
            UiCamera.rect = new Rect(0, 0, 1, 1);
        }
        else
        {
            playerIndex = playerCount;
            float width = 1f / numbersOfPlayers;
            float height = 1f;
            float x = playerIndex * width;
            float y = 0f;

            camera.rect = new Rect(x, y, width, height);
            UiCamera.rect = new Rect(x, y, width, height);
        }
        playerCount++;
    }

    public void MainMenuButton()
    {
        if (isNetworkGame) PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        playerCount--;
        if (playerCount <= 1)
        {
            EndGame(PhotonNetwork.LocalPlayer.NickName);
        }
    }
}
