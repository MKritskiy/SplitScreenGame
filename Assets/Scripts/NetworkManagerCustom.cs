using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerCustom : NetworkManager
{
    bool playerSpawned;
    bool playerConnected;
    public Transform[] spawnPoints;
    public Vector2 gameFieldSize = new Vector2(10f, 10f);
    public static int playerCount = 0;

    public struct PosMessage : NetworkMessage
    {
    }

    public struct StartGameMessage : NetworkMessage
    {
    }

    public struct EndGameMessage : NetworkMessage
    {
        public string winnerName;
    }

    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message)
    {
        GameObject go = SpawnPlayer(); //локально на сервере создаем gameObject
        NetworkServer.AddPlayerForConnection(conn, go); //присоеднияем gameObject к пулу сетевых объектов и отправляем информацию об этом остальным игрокам
    }

    public void OnStartGame(NetworkConnectionToClient conn, StartGameMessage message)
    {
        ServerChangeScene("GameSceneOnline");
    }

    public void OnEndGame(NetworkConnectionToClient conn, EndGameMessage message)
    {
        GameData.Instance.SetWinnerName(message.winnerName);
        ServerChangeScene("EndOnlineGameScene");
    }

    GameObject SpawnPlayer()
    {
        Transform spawnPoint;
        GameObject player;
        spawnPoints = GameManager.Instance?.spawnPoints;
        if (GameManager.Instance.spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[playerCount % spawnPoints.Length];
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

        return player;
    }

    public void ActivatePlayerSpawn()
    {
        PosMessage m = new PosMessage(); //создаем struct определенного типа, чтобы сервер понял к чему эти данные относятся
        NetworkClient.Send(m); //отправка сообщения на сервер
        playerSpawned = true;
    }

    public void StartGame()
    {
        StartGameMessage m = new StartGameMessage();
        NetworkClient.Send(m);
    }

    public void EndGame()
    {
        EndGameMessage m = new EndGameMessage();
        NetworkClient.Send(m);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter); //указываем, какой struct должен прийти на сервер, чтобы выполнился свапн
        NetworkServer.RegisterHandler<StartGameMessage>(OnStartGame);
        NetworkServer.RegisterHandler<EndGameMessage>(OnEndGame);

        Debug.Log("Server started");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        playerCount = 0;
        Debug.Log("Server stopped");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

    public override void Update()
    {
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client stopped");
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        if (SceneManager.GetActiveScene().name == "GameSceneOnline")
        {
            ActivatePlayerSpawn();
        }
    }

    public void ResetPlayerCount()
    {
        playerCount = 0;
    }

    private void OnClientsCountChanged(int oldValue, int newValue)
    {
        Debug.Log("Clients count changed: " + newValue);
    }
}
