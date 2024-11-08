using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public TextMeshProUGUI playerNumber;
    public Canvas canvas;
    public TextMeshProUGUI playerHP;
    public GameObject DieScreen;
    public bool isNetworkGame = false;

    private PlayerController father;
    private bool isOwned = false;

    void Start()
    {
        if (isNetworkGame)
        {
            
            isOwned = playerHP.GetComponentInParent<PlayerControllerOnline>().isOwned;
            if (isOwned)
            {
                father = playerHP.GetComponentInParent<PlayerControllerOnline>();
                playerNumber.text = (father as PlayerControllerOnline).playerName;     
            } else
            {
                gameObject.SetActive(false);
            }

        }
        else
        {
            father = playerHP.GetComponentInParent<PlayerControllerLocal>();
            playerNumber.text = (father as PlayerControllerLocal).playerName;

        }
        DieScreen.SetActive(false);
    }

    void Update()
    { 
        if (isNetworkGame) { playerHP.text = "HP: " + (father as PlayerControllerOnline)?.health; }
        else { playerHP.text = "HP: " + (father as PlayerControllerLocal)?.health; }
    }
}
