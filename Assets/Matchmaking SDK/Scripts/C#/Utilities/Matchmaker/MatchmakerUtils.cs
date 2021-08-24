using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine;
using System;

namespace BGGamesCore.Matchmaker.Utils
{
    [Serializable]
    public class PvPUserData
    {
        public int gaianiteCost;
        public string lastOpponentPlayfabID;
        public bool usedFreeTicket;
    }

    [Serializable]
    public class MatchResultData
    {
        public int MasterLeaderBoardMMR;
        public int LeaderboardCategoryPoints;
    }

    public static class MatchmakerUtils
    {
        private static Action onError;
        
        /// <summary>
        /// Fetch PVP data of player
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="OnError"></param>
        public static void GetPVPData(Action<PvPUserData> onSuccess, Action OnError)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            onError = OnError;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                
                FunctionName = "GetPVPData",
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            }, result =>
            {
                Debug.Log(result.FunctionName);
                Debug.Log(result.FunctionResult.ToString());
                var response = JsonConvert.DeserializeObject<PvPUserData>(result.FunctionResult.ToString());
                onSuccess(response);
            }, OnErrorShared);
        }
        
        /// <summary>
        /// Send PVP player data to Playfab
        /// </summary>
        /// <param name="pvpData"></param>
        /// <param name="onSuccess"></param>
        /// <param name="OnError"></param>
        public static void SetPvPData(PvPUserData pvpData, Action onSuccess, Action OnError)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            onError = OnError;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
               
                FunctionParameter = new{ lastOpponentPlayfabID = pvpData.lastOpponentPlayfabID },
                FunctionName = "SetPVPData",
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            }, result=> 
            {
                onSuccess();
            }, OnErrorShared);
        }

        /// <summary>
        /// Get result of the game.
        /// </summary>
        /// <param name="matchResult"></param>
        /// <param name="leaderboardCategoryName"></param>
        /// <param name="masterLeaderboardName"></param>
        /// <param name="playerId"></param>
        /// <param name="OnFinish"></param>
        /// <param name="OnError"></param>
        public static void GetMatchData(string matchResult, string leaderboardCategoryName, string masterLeaderboardName, string playerId, Action<MatchResultData> OnFinish, Action OnError)
        {


            Debug.Log("Result: " + matchResult + "CategoryLeaderboard: " + leaderboardCategoryName + "MasterLeaderboard: " + masterLeaderboardName +"PlayerId: "+playerId);
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            onError = OnError;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                
                FunctionParameter = new { result = matchResult, leaderboardCategory = leaderboardCategoryName, masterLeaderboardName = masterLeaderboardName, playerId = playerId },
                FunctionName = "OnPVPFinish",
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            }, dataReturn =>
            {
             //   Debug.LogError("res null?" + dataReturn == null);
    
                var response = JsonConvert.DeserializeObject<MatchResultData>(dataReturn.FunctionResult.ToString());
                OnFinish(response);
            }, OnErrorShared);
        }


        private static void OnErrorShared(PlayFabError error)
        {
         
            Debug.Log(error.GenerateErrorReport());

            if (onError != null)
            {
                onError();
                onError = null;
            }
        }
    }
}
