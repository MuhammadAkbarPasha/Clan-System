using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGamesCore.Matchmaker;

/// <summary>
/// Sample class for keeping track of player data and game states.
/// </summary>
public class GameDataManager : MonoBehaviour
{
    public PlayerMatchmakerData playerData;
    public PlayerMatchmakerData opponentData;

    public bool isDoingRevenge;
    public string revengeId;

    public static GameDataManager Instance
    {
        private set;
        get;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }    
    }

    public void ClearRevenge()
    {
        isDoingRevenge = false;
        revengeId = "";
    }

    public void UpdateData()
    { 
        playerData.avatarUrl = PlayFabManager.Instance.avatarUrl;
        playerData.name = PlayFabManager.Instance.username;
       // playerData.level = GetPlayerLevel
    }
}