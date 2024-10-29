using Mirror;
using UnityEngine;

public class NetworkManagerCustom : NetworkManager
{
    bool playerSpawned;
    bool playerConnected;
    public Transform[] spawnPoints;
    public Vector2 gameFieldSize = new Vector2(10f, 10f);
    public static int playerCount = 0;
    public struct PosMessage : NetworkMessage //наследуемся от интерфейса NetworkMessage, чтобы система поняла какие данные упаковывать
    {
        
    }


    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message)
    {
        GameObject go = SpawnPlayer(); //локально на сервере создаем gameObject
        NetworkServer.AddPlayerForConnection(conn, go); //присоеднияем gameObject к пулу сетевых объектов и отправляем информацию об этом остальным игрокам
    }

    GameObject SpawnPlayer()
    {
        
        Transform spawnPoint;
        GameObject player;
        if (spawnPoints.Length > 0)
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
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter); //указываем, какой struct должен прийти на сервер, чтобы выполнился свапн

        Debug.Log("Server started");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
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
        playerConnected = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !playerSpawned && playerConnected)
        {
            ActivatePlayerSpawn();
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client stopped");
    }
}
