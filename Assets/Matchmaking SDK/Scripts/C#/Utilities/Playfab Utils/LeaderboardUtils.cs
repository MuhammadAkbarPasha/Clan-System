using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;

namespace BGGamesCore.Matchmaker.Utils
{
    public class LeaderboardUtils
    {

        private static Action<PlayerLeaderboardEntry> onPlayerLeaderboardStatisticFetched;
        private static Action onLeaderboardUpdated;
        private static Action<List<PlayerLeaderboardEntry>> onLeaderboardFetched;
        private static Action onFailure;

        public static void GetPlayerLeaderboardStatistic(string leaderboardName, Action<PlayerLeaderboardEntry> successCallback, Action failureCallback = null)
        {
            onPlayerLeaderboardStatisticFetched = successCallback;
            onFailure = failureCallback;

            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = leaderboardName,
                MaxResultsCount = 1
            }, result => OnPlayerLeaderboardStatisticFetched(result.Leaderboard), OnFailure);
        }

        private static void OnPlayerLeaderboardStatisticFetched(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
        {
            Debug.Log("Leaderboard Fetched with " + playerLeaderboardEntries.Count + " entries");

            if (playerLeaderboardEntries.Count > 0)
            {
                if (onPlayerLeaderboardStatisticFetched != null)
                {
                    onPlayerLeaderboardStatisticFetched(playerLeaderboardEntries[0]);
                    onPlayerLeaderboardStatisticFetched = null;
                }
            }
            else
            {
                if (onFailure != null)
                {
                    onFailure();
                    onFailure = null;
                }
            }
        }

        public static void SetLeaderboardStatistic(string leaderboardName, int value, Action successCallback, Action failureCallback = null)
        {
            onLeaderboardUpdated = successCallback;
            onFailure = failureCallback;

            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> {
                        new StatisticUpdate {
                            StatisticName = leaderboardName,
                            Value = value
                        }
                    }
            }, result => OnLeaderboardUpdated(result), OnFailure);
        }

        private static void OnLeaderboardUpdated(UpdatePlayerStatisticsResult updateResult)
        {
            Debug.Log("Successfully submitted high score");

            if (onLeaderboardUpdated != null)
            {
                onLeaderboardUpdated();
                onLeaderboardUpdated = null;
            }
        }

        public static void RequestLeaderboard(string leaderboardName, int startPosition, int maxResultCount, PlayerProfileViewConstraints constraint, Action<List<PlayerLeaderboardEntry>> successCallback, Action failureCallback = null)
        {
            onLeaderboardFetched = successCallback;
            onFailure = failureCallback;
            constraint.ShowLinkedAccounts = true;
            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = leaderboardName,
                MaxResultsCount = maxResultCount,
                ProfileConstraints = constraint
            }, result => OnLeaderboardFetched(result.Leaderboard), OnFailure);
        }

        public static void RequestLeaderboardAroundPlayer(string leaderboardName, int maxResultCount, PlayerProfileViewConstraints constraint, Action<List<PlayerLeaderboardEntry>> successCallback, Action failureCallback = null)
        {
            onLeaderboardFetched = successCallback;
            onFailure = failureCallback;

            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = leaderboardName,
                MaxResultsCount = maxResultCount,
                ProfileConstraints = constraint
            }, result => OnLeaderboardFetched(result.Leaderboard), OnFailure);
        }

        private static void OnLeaderboardFetched(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
        {
            Debug.Log("Leaderboard Fetched with " + playerLeaderboardEntries.Count + " entries");
            Debug.Log("COming here again");
            if (playerLeaderboardEntries.Count > 0)
            {
                if (onLeaderboardFetched != null)
                {
                    onLeaderboardFetched(playerLeaderboardEntries);
                    onLeaderboardFetched = null;
                }
            }
            else
            {
                if (onFailure != null)
                {
                    onFailure();
                    onFailure = null;
                }
            }
        }

        private static void OnFailure(PlayFabError error)
        {
            Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());

            if (onFailure != null)
            {
                onFailure();
                onFailure = null;
            }
        }
    }
}
