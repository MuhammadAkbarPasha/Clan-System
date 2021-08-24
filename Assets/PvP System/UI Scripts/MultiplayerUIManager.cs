using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplayerUIManager : MonoBehaviour
{
    public static MultiplayerUIManager Instance;

    public GameObject TimerPanel;
    public Text TimerText;
    public TextMeshProUGUI titleText;
    public GameObject ReconnectingTextGameObject;
    public GameObject RestartButton; 


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateTimerPanelDisplay(bool display)
    {
        TimerPanel.SetActive(display);
    }

    public void UpdateTimerValue(int value)
    {
        if (value < 10)
            TimerText.text = "00:0" + value;
        else if (value <= 0)
            TimerText.text = "00:00"; 
        else
            TimerText.text = "00:" + value;
    }

    public void TurnObject(bool enableState)
    {
        if (enableState)
        {
            titleText.text = "Connection Lost!";
            TimerText.text = "Please connect to the internet \n and relaunch differences \n journey";
        }
        else
        {
            titleText.text = "Reconnecting";
            TimerText.text = "";
        }
        RestartButton.SetActive(enableState); 
    //    TurnObject(ReconnectingTextGameObject, !enableState);
    }

    public void TurnObject(GameObject gameObject, bool enableState)
    {
        gameObject.SetActive(enableState);
    }
}