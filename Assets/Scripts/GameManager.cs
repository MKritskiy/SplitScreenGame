using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // Префаб игрока
    public Transform[] spawnPoints; // Точки спавна игроков
    public Vector2 gameFieldSize = new Vector2(10f, 10f); // Размер игрового поля
    public GameObject cameraPrefab;
    public GameObject endGameScreen;
    public TextMeshProUGUI winnerNumber;
    public static int playerCount = 0;
    private int playerIndex = 0;

    void Start()
    {
        int numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 1);
        playerCount = 0;
        SpawnPlayers(numberOfPlayers);
        endGameScreen.SetActive(false);
    }

    public void EndGame(GameObject winner)
    {
        endGameScreen.SetActive(true);
        winnerNumber.text = winner.GetComponent<PlayerController>().playerName + " win!";
    }

    private void Update()
    {
    }
    void SpawnPlayers(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Transform spawnPoint;
            GameObject player;
            if (spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[i % spawnPoints.Length];
                player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-gameFieldSize.x / 2, gameFieldSize.x / 2),
                    0f,
                    Random.Range(-gameFieldSize.y / 2, gameFieldSize.y / 2)
                );
                player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            }

            player.GetComponent<PlayerController>().playerName = "Player " + (playerCount + 1);
            SetupCamera(numberOfPlayers, player.GetComponent<PlayerController>().cameraSpawnPoint);
        }
    }

    public void SetupCamera(int numbersOfPlayers, Transform cameraSpawnPoint)
    {
        Camera camera = cameraSpawnPoint.gameObject.GetComponentInChildren<Camera>();
        Camera UiCamera = camera.GetComponentsInChildren<Camera>()[1];
        UiCamera.enabled = true;
        camera.enabled = true;
        playerIndex = playerCount;
        float width = 1f / numbersOfPlayers;
        float height = 1f;
        float x = playerIndex * width;
        float y = 0f;

        camera.rect = new Rect(x, y, width, height);
        UiCamera.rect = new Rect(x, y, width, height);
        playerCount++;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
