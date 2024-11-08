using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerCustom : NetworkManager
{
    bool playerSpawned;
    bool playerConnected;
    public Transform[] spawnPoints;
    public Vector2 gameFieldSize = new Vector2(10f, 10f);
    public static int playerCount = 0;
    //public ClientCounter clientCounter;
    public string winnerName;
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
        winnerName = message.winnerName;

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

        //GameObject clientCounterObject = new GameObject("ClientCounter");
        //clientCounterObject.AddComponent<NetworkIdentity>();
        //clientCounter = clientCounterObject.AddComponent<ClientCounter>();

        //NetworkServer.Spawn(clientCounterObject);
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
        //clientCounter.IncrementClientsCount();
        //Debug.Log("Client connected. Total clients: " + clientCounter.clientsCount);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        //clientCounter.DecrementClientsCount();
        //Debug.Log("Client disconnected. Total clients: " + clientCounter.clientsCount);
    }

    public override void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0) && !playerSpawned && playerConnected)
        //{
        //    ActivatePlayerSpawn();
        //}
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
        // Этот метод вызывается каждый раз, когда значение clientsCount изменяется
        Debug.Log("Clients count changed: " + newValue);
    }
}
