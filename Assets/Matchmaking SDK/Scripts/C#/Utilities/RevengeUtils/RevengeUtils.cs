using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BGGamesCore.Matchmaker.Utils
{
    [Serializable]
    public class RevengeData
    {
        public int maxTicketCount;
        public int currentTicketCount;
        public string[] revengeIDs;
    }

    public static class RevengeUtils
    {
        private static Action onError;
        /// <summary>
        /// Updates the revenge data of both the players passed in the parameters.
        /// Player's PlayFabId is added to Opponent's if player won.
        /// Opponent's PlayFabId is not removed from Player's revenge list if player lost.
        /// </summary>
        /// <param name="PlayerPlayFabId"></param>
        /// <param name="OpponentPlayFabId"></param>
        /// <param name="didWin"></param>
        /// <param name="onSuccess"></param>
        /// <param name="OnError"></param>
        public static void UpdateRevengeList(string PlayerPlayFabId, string OpponentPlayFabId, bool didWin, Action onSuccess, Action OnError)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            onError = OnError;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                SpecificRevision = 2133,
                FunctionParameter = new { PlayerPlayFabId, OpponentPlayFabId, didWin },
                FunctionName = "UpdateRevengeList",
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            }, result =>
            {
                onSuccess();
            }, OnErrorShared);
        }

        /// <summary>
        /// Fetches revenge data
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="OnError"></param>
        public static void GetRevengeData(Action<RevengeData> onSuccess, Action OnError)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }

            onError = OnError;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                SpecificRevision = 2133,
                FunctionName = "GetRevengeData",
                GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
            }, result =>
            {
                var response = JsonConvert.DeserializeObject<RevengeData>(result.FunctionResult.ToString());
                onSuccess(response);
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

