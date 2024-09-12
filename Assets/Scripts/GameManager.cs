using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // Префаб игрока
    public Transform[] spawnPoints; // Точки спавна игроков
    public Vector2 gameFieldSize = new Vector2(10f, 10f); // Размер игрового поля
    public GameObject cameraPrefab;

    public static int playerCount = 0;
    private int playerIndex = 0;

    void Start()
    {
        int numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 1);
        playerCount = 0;
        SpawnPlayers(numberOfPlayers);
    }

    public static void EndGame(GameObject winner)
    {

    }

    void SpawnPlayers(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Vector3 spawnPosition;
            if (spawnPoints.Length > 0)
            {
                spawnPosition = spawnPoints[i % spawnPoints.Length].position;
            }
            else
            {
                spawnPosition = new Vector3(
                    Random.Range(-gameFieldSize.x / 2, gameFieldSize.x / 2),
                    0f,
                    Random.Range(-gameFieldSize.y / 2, gameFieldSize.y / 2)
                );
            }

            GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            SetupCamera(numberOfPlayers, player.GetComponent<PlayerController>().cameraSpawnPoint);
        }
    }

    public void SetupCamera(int numbersOfPlayers, Transform cameraSpawnPoint)
    {
        GameObject cameraObject = Instantiate(cameraPrefab, cameraSpawnPoint.position, cameraSpawnPoint.rotation, cameraSpawnPoint.transform);
        Camera camera = cameraObject.GetComponent<Camera>();
        playerIndex = playerCount;
        float width = 1f / numbersOfPlayers;
        float height = 1f;
        float x = playerIndex * width;
        float y = 0f;

        camera.rect = new Rect(x, y, width, height);
        playerCount++;
    }
}
