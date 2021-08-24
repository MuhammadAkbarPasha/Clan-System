using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using System.ComponentModel;
/// <summary>
/// 
/// </summary>
public class LeaderboardSystem : MonoBehaviour
{

    /// <summary>
    /// Static instance of LeaderboardSystem
    /// </summary>
    public static LeaderboardSystem Instance;

    /// <summary>
    /// LeaderboardData object holding local leaderboard
    /// </summary>
    public LeaderboardData localLeaderboardData;
    /// <summary>
    /// LeaderboardData object holding global leaderboard
    /// </summary>
    public LeaderboardData globalLeaderboardData;
    /// <summary>
    /// LeaderboardData object holding friends leaderboard
    /// </summary>
    public LeaderboardData friendsLeaderboardData;


    public void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Get Gloabal Leaderboard from playfab server
    /// </summary>
    public void GetLeaderboardTop100() 
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetLeaderboardTop100",
            GeneratePlayStreamEvent = true,

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Get Local Leaderboard from playfab server
    /// </summary>
    public void GetLocalLeaderboard()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetLocalLeaderboard",
            FunctionParameter = new
            {
                LeaderboardId=PlayFabManager.Instance.playerCountry.CountryLevel
            },
            GeneratePlayStreamEvent = true,

        }, CloudScriptSuccess, CloudScriptFailure);
    }

/// <summary>
/// Get leaderboard of friends from playfab server
/// </summary>
    public void GetLeaderboardOfFriends()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetLeaderboardOfFriends",
            FunctionParameter = new
            {
                LeaderboardId = PlayFabManager.Instance.playerCountry.CountryLevel
            },
            GeneratePlayStreamEvent = true,

        }, CloudScriptSuccess, CloudScriptFailure);
    }


 /// <summary>
    /// callback on successful response from the server
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    public void CloudScriptSuccess(ExecuteCloudScriptResult result) /// Made CHanges Here 9/11/20
    {
        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);

        switch (result.FunctionName)
        {
            case "GetLeaderboardTop100":
                {
                    // Debug.Log(result.FunctionResult);
                    if (result.FunctionResult != null)
                    {
                        globalLeaderboardData = JsonUtility.FromJson<LeaderboardData>(result.FunctionResult.ToString());
                        //UiController.Instance.PopulateGlobalLeaderboard();
                    }
                }
                break;
            case "GetLocalLeaderboard": 
                {
                    if (result.FunctionResult != null)
                    {
                        localLeaderboardData = JsonUtility.FromJson<LeaderboardData>(result.FunctionResult.ToString());
                        //UiController.Instance.PopulateLocalLeaderboard();
                    }
                    
                }
                break;
            case "GetLeaderboardOfFriends": 
                {
                    if (result.FunctionResult != null)
                    {
                        friendsLeaderboardData = JsonUtility.FromJson<LeaderboardData>(result.FunctionResult.ToString());
                        //UiController.Instance.PopulateFriendsLeaderboard();
                       // PlayPabUiController.Instance.PopulateFriendsLeaderboard();
                    }
                    
                }
                break;
        }
    }

    /// <summary>
    /// Clear all data on sign out or for any other purpose
    /// </summary>
    public void ClearData()
    {
    }

    /// <summary>
    /// cloud script function failure callback
    /// </summary>
    public void CloudScriptFailure(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
}