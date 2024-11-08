using Mirror;
using UnityEngine;

public class GameData : NetworkBehaviour
{
    public static GameData Instance;
    public string winnerName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetWinnerName(string name)
    {
        winnerName = name;
    }

    public string GetWinnerName()
    {
        return winnerName;
    }
}
