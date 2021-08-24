using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using Chamoji.Social;
using Chamoji;

//Util class taken from Murasaki7
public static class ArenaUtilities
{
    public static int HARD_WIN_VALUE = 30;
    public static int HARD_LOSE_VALUE = -20;
    public static int REGULAR_WIN_VALUE = 15;
    public static int REGULAR_LOSE_VALUE = -10;
                        
    public enum ArenaGameEndType
    {
        HARD_WIN,
        HARD_LOSE,
        REGULAR_WIN,
        REGULAR_LOSE
    }

    private static Action onError;
    private const string ARENA_ENTRY_KEY_CONDITION = "pvpArenaKey";
    private const string ARENA_REVENGE_DATA = "pvpRevengeData";

    public static void GetUserArenaEntryConditionDetails(Action<ArenaModel> onFetchFinished = null)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayerDataAPI.GetSpecificData<ArenaModel>(FakePlayerHandler.Instance.PlayerData.playFabId, ARENA_ENTRY_KEY_CONDITION,
                arenaPlayerModel => {
            if (!PlayerDataAPI.IsDataValid(ARENA_ENTRY_KEY_CONDITION))
            {
                TitleDataAPI.GetData<ArenaModel>(ARENA_ENTRY_KEY_CONDITION,
                    arenaTitleModel =>
                    {
                        onFetchFinished(arenaTitleModel);
                    });
            }
            else
            {
                onFetchFinished(arenaPlayerModel);
            }
        });
    }

    public static void GetUserRevengeData(Action<ArenaRevengeModel> onFetchFinished = null)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayerDataAPI.GetData<ArenaRevengeModel>(ARENA_REVENGE_DATA,
            arenaPlayerModel => {
                if (!PlayerDataAPI.IsDataValid(ARENA_REVENGE_DATA))
                {
                    TitleDataAPI.GetData<ArenaRevengeModel>(ARENA_REVENGE_DATA,
                            arenaTitleModel =>
                            {
                                onFetchFinished(arenaTitleModel);
                            });
                }
                else
                {
                    onFetchFinished(arenaPlayerModel);
                }
            });
    }

    public static void SetRevengeData(ArenaRevengeModel revengeModel, Action onSuccess)
    {
        PlayerDataAPI.UpdateData<ArenaRevengeModel>(ARENA_REVENGE_DATA, revengeModel, true);
        PlayerDataAPI.PostData(onSuccess, ARENA_REVENGE_DATA);
    }

    public static void SetUserArenaEntryConditionDetails(ArenaModel arenaModel, Action onSuccess)
    {
        PlayerDataAPI.UpdateData<ArenaModel>(ARENA_ENTRY_KEY_CONDITION, arenaModel, true);
        PlayerDataAPI.PostData(onSuccess, ARENA_ENTRY_KEY_CONDITION);
    }

    public static void GetUserArenaDetails(string playFabId, Action<ArenaPlayer> OnReceivedUserArenaDetails, Action OnError)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        onError = OnError;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "GetUserArenaDetails",
            FunctionParameter = new { PlayFabId = playFabId },
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, result =>
        {
            var response = JsonConvert.DeserializeObject<ArenaPlayer>(result.FunctionResult.ToString());
            OnReceivedUserArenaDetails(response);
        }, OnErrorShared);
    }

    public static void RerollArena(Action onSuccess, Action<string> onError)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "PVPReRollEnemy",
            GeneratePlayStreamEvent = true

        }, result =>
        {
            Dictionary<string,string> resultString = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
            string error = resultString["err"];
            if (string.IsNullOrEmpty(error))
            {
                if (onSuccess != null)
                {
                    onSuccess();
                }
            }
            else
            {
                if(onError != null)
                {
                    onError(error);
                }
            }
            //result.FunctionResult.ToString();
        }, OnErrorShared);
    }

    /*
    public static void UpdatePlayerArenaMMR(string playerPlayFabId, string opponentPlayFabId, ArenaResultScreen.ArenaGameResultType gameResult, int maxDamage, bool isRevenge, Action<ArenaUpdateMMRResult> OnUpdateArenaMMR, Action OnError)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        onError = OnError;
        bool ForceWeekend = ServerTimeUtility.Instance.IsWeekend;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerArenaMMR",
            FunctionParameter = new { PlayerPlayFabId = playerPlayFabId, OpponentPlayFabId = opponentPlayFabId, ResultType = gameResult.ToString(), MaxDamage = maxDamage, IsRevenge = isRevenge, DebugWeekend = ForceWeekend },
            GeneratePlayStreamEvent = true // Optional - Shows this event in PlayStream
        }, result =>
        {
            var response = JsonConvert.DeserializeObject<ArenaUpdateMMRResult>(result.FunctionResult.ToString());
            OnUpdateArenaMMR(response);
        }, OnErrorShared);
    }
    */
    public static void ConsumeRevengeTicket(Action<int> onSuccess)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "ConsumeRevengeTicket",
            GeneratePlayStreamEvent = true
        }, result =>
        {
            var response = JsonConvert.DeserializeObject<int>(result.FunctionResult.ToString());
            onSuccess(response);
        }, OnErrorShared);
    }

    public static void ClearLastOpponentData(Action onSuccess)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "ClearLastOpponent",
            GeneratePlayStreamEvent = true
        }, result =>
        {
            if (onSuccess != null)
            {
                onSuccess();
            }
        }, OnErrorShared);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());

        if(onError != null)
        {
            onError();
            onError = null;
        }
    }

    /*
    public static ArenaGameEndType GetGameLoseType(LeaderboardArenaRank playerArenaRank, LeaderboardArenaRank opponentArenaRank)
    {
        if (playerArenaRank <= opponentArenaRank)
        {
            return ArenaGameEndType.REGULAR_LOSE;
        }
        else
        {
            return ArenaGameEndType.HARD_LOSE;
        }

    }
    */

        /*
    public static void Print(string message)
    {
#if UNITY_EDITOR || DEBUG
            message.Print(Color.cyan);
#endif
    }
    */
}

public struct ArenaUpdateMMRResult
{
    public int CurrentGainedMMR;
    public int CurrentMMRWeekly;
    public int CurrentMMRMonthly;
    public int CurrentWinStreak;
    public int CurrentWinStreakHistorical;
    public int CurrentBattles;
    public int CurrentWins;
    public int CurrentLosses;
    public int CurrentMaxDamage;
    public int CurrentBonusMMR;
}

[Serializable]
public class ArenaModel
{
    public int gaianiteCost;
    public string lastOpponentPlayfabID;
    public bool usedFreeTicket;
}

[Serializable]
public class ArenaRevengeModel
{
    public int maxTicketCount;
    public int currentTicketCount;
    public string[] revengeIDs;
}

/*
public class RevengePlayerData
{
    //public string PlayerID;
    //public string PlayerAvatarUrl;
    //public string PlayerName;
    //public int MMR;
    public LeaderboardEntry leaderboardEntryData;
    public PartyObject partyObject;
    public int powerPartyLevel;
    public bool isInit;
}
*/