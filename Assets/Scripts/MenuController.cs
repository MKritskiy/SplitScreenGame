using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Slider playerCountSlider;
    public TextMeshProUGUI playesCountText;
    public TMP_InputField hostAddressInput;

    public void ChangeCountWithSlider()
    {
        playesCountText.text = Mathf.RoundToInt(playerCountSlider.value) + " Players";
    }

    public void StartGame()
    {
        int numberOfPlayers = Mathf.RoundToInt(playerCountSlider.value);
        PlayerPrefs.SetInt("NumberOfPlayers", numberOfPlayers);
        SceneManager.LoadScene("GameScene"); // Загрузите вашу основную сцену
    }
    public void HostGame()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    public void JoinGame()
    {
        string hostAddress = hostAddressInput.text;
        PlayerPrefs.SetString("HostAddress", hostAddress);
        SceneManager.LoadScene("LobbyScene");
    }
}
