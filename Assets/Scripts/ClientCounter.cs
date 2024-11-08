using Mirror;
using UnityEngine;

public class ClientCounter : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnClientsCountChanged))]
    public int clientsCount = 0;

    private void OnClientsCountChanged(int oldValue, int newValue)
    {
        Debug.Log("Clients count changed: " + newValue);
    }

    [Server]
    public void IncrementClientsCount()
    {
        clientsCount++;
    }

    [Server]
    public void DecrementClientsCount()
    {
        clientsCount--;
    }
}
