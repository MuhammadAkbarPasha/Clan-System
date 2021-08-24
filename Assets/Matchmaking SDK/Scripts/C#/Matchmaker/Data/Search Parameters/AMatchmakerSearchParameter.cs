 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using UnityEditor;
namespace BGGamesCore.Matchmaker
{
    public abstract class AMatchmakerSearchParameter : ScriptableObject
    {
        /// <summary>
        /// Names of the leaderboards that will be searched.
        /// </summary>
        [SerializeField]
        protected string[] leaderboardNames;
        public string[] LeaderboardNames
        {
            get
            {
                return leaderboardNames;
            }
        }
        /// <summary>
        /// Player's rank when matchmaker initialized
        /// </summary>
        public int PlayerRankOnInitialization
        {
            private set; get;
        }
        /// <summary>
        /// Current tier the matchmaking system is looking at.
        /// </summary>
        public int currentSearchTier;
        /// <summary>
        /// Integer equivalent of a search parameter's search mode enum
        /// </summary>
        public int currentSearchMode;
        /// <summary>
        /// Keeps track how many times the search parameter changed.
        /// </summary>
        public int searchParameterChangeCounter = -1;
        /// <summary>
        /// Returns the enum size of a particular search parameter
        /// </summary>
        protected virtual int SearchEnumSize
        {
            get;
            set;
        }

        public virtual string GetPlayerCurrentLeaderBoardName(int tier)
        {
            return "";
        }
        public virtual MmrRanges GetMmrRange(string ArenaName)
        {
            return null;
            
        }
        /// <summary>
        /// Returns the specific tier of the player depending on the input MMR
        /// </summary>
        /// <param name="mmr">Player MMR</param>
        /// <returns></returns>
        public virtual int GetTier(int mmr)
        {
            return 0;
        }
        /// <summary>
        /// Returns string equivalent of the tier
        /// </summary>
        /// <returns></returns>
        public virtual string GetTierString(int tier)
        {
            return "";
        }
        /// <summary>
        /// Checks if the search rank was incremented
        /// </summary>
        protected bool searchRankIncremented;
        /// <summary>
        /// Pointer to the player's data in the matchmaking system
        /// </summary>
        protected PlayerMatchmakerData playerData;
        /// <summary>
        /// Method called when player is null or empty in the Matchmaking system
        /// </summary>
        public abstract void OnPlayerNullOrEmpty();

        /// <summary>
        /// Method called when a player has passed all the search conditions
        /// Use this to load the needed data from the opposing player.
        /// </summary>
        public abstract void OnPlayerFound(PlayerLeaderboardEntry opponentLeaderboardEntry, bool previousPlayer, System.Action<bool, PlayerMatchmakerData> onFinished);

        /// <summary>
        /// Checks the current state of the Matchmaker and changes the parameters according to the specified conditions
        /// </summary>
        /// <param name="playersInCurrentTier"></param>
        public abstract void ChangeSearchParameters(List<PlayerLeaderboardEntry> playersInCurrentTier);
        /// <summary>
        /// Initializes the current search parameter
        /// </summary>
        public virtual void Initialize(PlayerMatchmakerData playerData, Dictionary<int, MatchmakingLeaderboardEntry> matchmakingLeaderboardEntries)
        {
            this.playerData = playerData;
            ResetSearchTier(playerData.mmr);

            for(int i = 0; i < leaderboardNames.Length; i++)
            {
                MatchmakingLeaderboardEntry currentMatchmakingLeaderboardEntry = matchmakingLeaderboardEntries[i] = new MatchmakingLeaderboardEntry();
                currentMatchmakingLeaderboardEntry.leaderboardName = leaderboardNames[i];
                currentMatchmakingLeaderboardEntry.LeaderboardCategory = i;
            }
        }
        /// <summary>
        /// Resets the search parameters. Override to add additional variables to reset when the Matchmaking system calls this method.
        /// </summary>
        public virtual void ResetSearchParameters()
        {
            currentSearchMode = 0;
            searchParameterChangeCounter = -1;
            currentSearchTier = 0;
        }
        /// <summary>
        /// Resets tier to player rank
        /// </summary>
        public void ResetSearchTier(int playerMMR)
        {
            this.PlayerRankOnInitialization = GetTier(playerMMR);
        }
    }
}
