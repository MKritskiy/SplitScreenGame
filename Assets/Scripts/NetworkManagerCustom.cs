using Mirror;
using UnityEngine;

public class NetworkManagerCustom : NetworkManager
{
    bool playerSpawned;
    bool playerConnected;
    public Transform[] spawnPoints;
    public Vector2 gameFieldSize = new Vector2(10f, 10f);
    public static int playerCount = 0;
    public struct PosMessage : NetworkMessage //����������� �� ���������� NetworkMessage, ����� ������� ������ ����� ������ �����������
    {
        
    }


    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message)
    {
        GameObject go = SpawnPlayer(); //�������� �� ������� ������� gameObject
        NetworkServer.AddPlayerForConnection(conn, go); //������������ gameObject � ���� ������� �������� � ���������� ���������� �� ���� ��������� �������
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
        PosMessage m = new PosMessage(); //������� struct ������������� ����, ����� ������ ����� � ���� ��� ������ ���������
        NetworkClient.Send(m); //�������� ��������� �� ������
        playerSpawned = true;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter); //���������, ����� struct ������ ������ �� ������, ����� ���������� �����

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
