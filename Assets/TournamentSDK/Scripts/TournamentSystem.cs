using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using System.Linq;
using StoreSystem;
using System;
//using WebSocketSharp;

public class TournamentSystem : MonoBehaviour
{
    [SerializeField]
    [Header("Time In Seconds Before Personal Tournament Ends")]
    int personalTournamentEndTimeInSeconds;

    [SerializeField]
    [Header("Time In Seconds Before Team Event Ends")]
    int TeamEventEndTimeInSeconds;

    [SerializeField]
    [Header("Time In Seconds Before Team Event Starts")]
    int TeamEventStartTimeInSeconds;

    public Tournament personalTournament;
    public TeamEvent teamEvent = new TeamEvent();

    public static TournamentSystem Instace;
    public PlayerStat thisPlayerInstanceInGroupTournament = new PlayerStat();

    public SinglePlayerData thisPlayerInstanceInStarTournament = new SinglePlayerData();

    private Tournament dummyTournament1;
    private Tournament dummyTournament2;

    private TeamEvent dummyTeamEvent1;

    public CustomData productAwarded;

    public void SetPlayerLevel(int newLevel)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "SetPlayerLevel",
            FunctionParameter = new
            {
                level = newLevel
            },

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    public void Awake()
    {
        Instace = this;
    }

    /// <summary>
    ///  Get Team Event End Time In Seconds
    /// </summary>
    /// 
    [ContextMenu("GetTeamEventEndTime")]
    public void GetTeamEventEndTime()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetTeamEventEndTime",

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Get Team Event Start Time In Seconds
    /// </summary>
    /// 
    [ContextMenu("GetTeamEventStartTime")]
    public void GetTeamEventStartTime()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetTeamEventStartTime",

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Get Personal Tournament Time In Seconds
    /// </summary>
    /// 
    [ContextMenu("GetPersonalTournamentEndTime")]
    public void GetPersonalTournamentEndTime()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetPersonalTournamentEndTime",

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Get Personal Tournament data from Playfab server for active/this player
    /// </summary>
    /// 
    [ContextMenu("GetPersonalTournament")]
    public void getPersonalTournament()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetPersonalMiniTournamentForPlayer",

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Add value on the  playfab server for this Personal Tournament Leaderboard
    /// </summary>
    public void AddValueToPersonalTournament(int starsToAdd)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "addValueToPersonalTournament",
            FunctionParameter = new
            {
                StarsToAdd = starsToAdd
            },

        }, CloudScriptSuccess, CloudScriptFailure);
    }

    #region Team_Event
    /// <summary>
    /// Add value on the  playfab server for this Personal Tournament Leaderboard
    /// </summary>
    public void ClaimTeamEventChest()
    {
        if (String.IsNullOrEmpty(PlayFabManager.Instance.currentMember.MemberGroup.Id))
            return;

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "ClaimTeamEventChest",
            FunctionParameter = new
            {
                groupId = PlayFabManager.Instance.currentMember.MemberGroup.Id
            },
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Add value on the  playfab server for this Personal Tournament Leaderboard
    /// </summary>
    public void GetTeamEvent()
    {
            //Debug.LogError("GetTeamEvent1111111");
            if (string.IsNullOrEmpty(PlayFabManager.Instance.currentMember.MemberGroup.Id))
            {
                Invoke(nameof(GetTeamEvent), 1);
                //PlayPabUiController.Instance.isTeamChestStarted = false;
                //PlayPabUiController.Instance.ShowTeamChestHomeButton(false);
                return;
            }

            //Debug.LogError("GetTeamEvent2222222");
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
            {
                FunctionName = nameof(GetTeamEvent),
                FunctionParameter = new
                {
                    groupId = PlayFabManager.Instance.currentMember.MemberGroup.Id
                },

            }, CloudScriptSuccess, CloudScriptFailure);
        
    }

    /// <summary>
    /// Add value on the  playfab server for this Personal Tournament Leaderboard
    /// </summary>
    public void AddScoreToTeamEventAndPlyer(int StarsToAdd)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "AddScoreToTeamEventAndPlyer",
            FunctionParameter = new
            {
                groupId = PlayFabManager.Instance.currentMember.MemberGroup.Id,
                stars = StarsToAdd
            },
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Add value on the  playfab server for this Personal Tournament Leaderboard
    /// </summary>
    #endregion
    public void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);

        switch (result.FunctionName)
        {
            case "GetPersonalMiniTournamentForPlayer":
                {
                    //PlayPabUiController.Instance.statPersonalTournamentLoaderObj.SetActive(false);
                    //PlayPabUiController.Instance.tournamentLoaderObj.SetActive(false);
                    if (result.FunctionResult != null)
                    {
                        dummyTournament1 = JsonUtility.FromJson<Tournament>(result.FunctionResult.ToString());
                        List<SinglePlayerData> SortedList = dummyTournament1.singlePlayerData.OrderByDescending((o) => (o.Value)).ToList();
                        personalTournament.singlePlayerData = SortedList;
                        SetStarTournamnetThisPlayer();
                    }
                    //PlayPabUiController.Instance.PopulatePersonalTournament();
                }
                break;
            case "GetPersonalTournamentEndTime":
                {
                    personalTournamentEndTimeInSeconds = int.Parse(result.FunctionResult.ToString());
                    //chirag.......
                    //PlayPabUiController.Instance.totalSecondOfPersonalTournament = personalTournamentEndTimeInSeconds;
                    //PlayPabUiController.Instance.GetPersonalTournamentTime();
                }
                break;
            case "addValueToPersonalTournament":
                {
                    getPersonalTournament();
                }
                break;
            case "GetTeamEventStartTime":
                {
                    TeamEventStartTimeInSeconds = int.Parse(result.FunctionResult.ToString());
                    //PlayPabUiController.Instance.totalSecondOfTeamChestStart = TeamEventStartTimeInSeconds;
                    //PlayPabUiController.Instance.GetTeamChestStartTime();
                }
                break;
            case "GetTeamEventEndTime":
                {
                    TeamEventEndTimeInSeconds = int.Parse(result.FunctionResult.ToString());
                    //PlayPabUiController.Instance.totalSecondOfTeamChest = TeamEventEndTimeInSeconds;
                    //PlayPabUiController.Instance.GetTeamChestTime();
                }
                break;
            case "AddScoreToTeamEventAndPlyer":
                {
                    GetTeamEvent();
                }
                break;
            case nameof(GetTeamEvent):
                {
                    //PlayPabUiController.Instance.teamChestLoaderObj.SetActive(false);
                    if (result.FunctionResult != null)
                    {
                        //if (result.FunctionResult.ToString() == "Not Available Today")
                        //{
                        //PlayPabUiController.Instance.ShowTeamChestHomeButton(false);
                        //  return;
                        //}

                        //if (//PlayPabUiController.Instance.totalSecondOfTeamChest == 0)
                        //{
                        GetTeamEventEndTime();
                        //}
                        //if (//PlayPabUiController.Instance.totalSecondOfTeamChestStart == 0)
                        //{
                        GetTeamEventStartTime();
                        //}
                        teamEvent = JsonUtility.FromJson<TeamEvent>(result.FunctionResult.ToString());
                        List<PlayerStat> SortedList = teamEvent.PlayerStats.OrderByDescending((o) => (o.Stars)).ToList();
                        teamEvent.PlayerStats = SortedList;
                        SetThisPlayer();
                        //PlayPabUiController.Instance.populateGroupTournament();

                        if (thisPlayerInstanceInGroupTournament != null)
                        {
                            //Debug.LogError("thisPlayerInstanceInGroupTournament:" + !thisPlayerInstanceInGroupTournament.Claimed);
                            //PlayPabUiController.Instance.TeamEventClaimButton(teamEvent.GoalAchieved && !thisPlayerInstanceInGroupTournament.Claimed);
                        }
                        else
                        {
                            //PlayPabUiController.Instance.ShowTeamChestHomeButton(false);
                        }
                    }
                }
                break;
            case "ClaimTeamEventChest":
                {
                    //PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
                    if (result.FunctionResult != null)
                    {
                        productAwarded = JsonUtility.FromJson<CustomData>(result.FunctionResult.ToString());
                        Debug.Log(result.FunctionResult);
                    }
                    TradeSystemController.Instance.LoadVirtualCurrency();
                    //PlayPabUiController.Instance.TeamChestPopupOpen();
                }
                break;
        }
    }

    public void SetThisPlayer()
    {
        thisPlayerInstanceInGroupTournament = teamEvent.PlayerStats.Find((ob) => ob.isCurrentPlayer == true);
    }

    public void SetStarTournamnetThisPlayer()
    {
        thisPlayerInstanceInStarTournament = personalTournament.singlePlayerData.Find((ob) => ob.IsthisCurrentPlayer == true);
    }

    public void CloudScriptFailure(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
}