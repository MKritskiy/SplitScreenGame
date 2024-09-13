using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public TextMeshProUGUI playerNumber;
    public Canvas canvas;
    public TextMeshProUGUI playerHP;
    private PlayerController father;
    void Start()
    {
        father = playerHP.GetComponentInParent<PlayerController>();
        playerNumber.text = father.playerName;
    }

    void Update()
    {
        playerHP.text = "HP: " + father.health;
    }
}
