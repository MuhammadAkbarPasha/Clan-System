using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using System;
using BGGamesCore.Matchmaker.Utils;
using System.Linq;
using System.Diagnostics;
using PlayFab.AuthenticationModels;

namespace BGGamesCore.Matchmaker
{
    [Serializable]
    public class MmrRanges
    {
        public string MmrArena;
        public int LowerRange;
        public int UpperRange;

    };
    public class MatchmakingLeaderboardEntry
    {
        /// <summary>
        /// Name of the leaderboard this is pointing to. 
        /// </summary>
        public string leaderboardName = "";
        /// <summary>
        /// Determines where the request will start
        /// </summary>
        public int startPosition = 0;
        /// <summary>
        /// Determines the max result count of the current leaderboard
        /// </summary>
        public int maxResultCount = 100;
        /// <summary>
        /// Checks if object has been initialized
        /// </summary>
        public bool isInitialized;
        /// <summary>
        /// Integer equivalent of the current leaderboard category. eg (enum => 0 = Bronze, 1 = Silver, etc...)
        /// </summary>
        public int LeaderboardCategory;
        /// <summary>
        /// Leaderboard entries in a specific category
        /// </summary>
        public List<PlayerLeaderboardEntry> LeaderboardEntries;
    }

    public class MatchmakingSystem : MonoBehaviour
    {
        [Header("Ainame List")]
        public int currentRandomInt;
        public string[] dummyNames;

        public int mmrRange;
        /// <summary>
        /// Singleton implementation
        /// </summary>
        public static MatchmakingSystem Instance
        {
            private set; get;
        }
        /// <summary>
        /// Search conditions that will be used by the Matchmaking System
        /// </summary>
        [SerializeField]
        private AMatchmakerSearchParameter matchmakerSearchParameters = null;
        public AMatchmakerSearchParameter GetSearchParameters
        {
            get { return matchmakerSearchParameters; }
        }
        /// <summary>
        /// Name of the unfiltered master leaderboard in Playfab.
        /// </summary>
        public string masterLeaderboardName;

        private Dictionary<int, MatchmakingLeaderboardEntry> matchmakingLeaderboardEntries;
        /// <summary>
        /// Invoked when opponent has been found
        /// </summary>
        private Action<PlayerMatchmakerData> onFoundMatch = null;
        /// <summary>
        /// Pointer to the current player data
        /// </summary>
        public PlayerMatchmakerData playerData;
        /// <summary>
        /// Collection of player IDs that have met the search criteria and can fight the current player.
        /// </summary>
        public List<string> encounteredValidIDs = null;
        /// <summary>
        /// Specifies the next element the matchmaker should access.
        /// </summary>
        public int continueIndex = -1;
        /// <summary>
        /// Checks if a player is currently being loaded. 
        /// If true, it yields the findPlayerRoutine until a player has been found.
        /// </summary>
        public bool loadingPlayer = false;
        /// <summary>
        /// Toggled if a match was found or not
        /// </summary>
        public bool FoundMatch;

        /// <summary>
        /// Main routine responsible for searching players
        /// </summary>
        private Coroutine findPlayerRoutine = null;

        /// <summary>
        /// Checks if the player encountered a player before the search. This prevents players from skipping players they don't like.
        /// </summary>
        private bool hasPreviousRecord;
        /// <summary>
        /// Index of the opponent the player last encountered. 
        /// ContinueIndex jumps to this index if there is an opponent.
        /// </summary>
        private int lastOpponentIndex = -1;
        private string lastOpponentPlayfabId;
        /// <summary>
        /// Playfab Id of the last opponent of the player
        /// </summary>
        public string m_LastOpp;
        public string LastOpponentPlayfabId
        {
            set
            {
                lastOpponentPlayfabId = value;
                hasPreviousRecord = string.IsNullOrEmpty(lastOpponentPlayfabId) == false;
                m_LastOpp = value;
            }

            get
            {
                return lastOpponentPlayfabId;
            }
        }
        /// <summary>
        /// Returns the current leaderboard name according to the search tier
        /// </summary>
        public string CurrentLeaderboardName
        {
            get
            {
                /*    int currentSearchTier = matchmakerSearchParameters.currentSearchTier;
                    return matchmakingLeaderboardEntries[currentSearchTier].leaderboardName;
                */
                return masterLeaderboardName;
            }
        }

        private List<PlayerLeaderboardEntry> GetPlayersInTier(int tier)
        {
            if (matchmakingLeaderboardEntries.ContainsKey(tier) == false)
                return null;
            MatchmakingLeaderboardEntry currentMatchmakingEntry = matchmakingLeaderboardEntries[tier];
            if (currentMatchmakingEntry.LeaderboardEntries.Count < 1)
                return null;
            return currentMatchmakingEntry.LeaderboardEntries;
        }

        private bool isInit = false;
        #region Unity methods
        private void Awake()
        {
            //Create Singleton instance
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
        }

        private void Start()
        {
            matchmakingLeaderboardEntries = new Dictionary<int, MatchmakingLeaderboardEntry>();
            encounteredValidIDs = new List<string>();

            matchmakerSearchParameters = Instantiate(matchmakerSearchParameters);
        }
        #endregion

        /// <summary>
        /// Begins the matchmaking process
        /// </summary>
        /// <param name="onFinish">Returns the Player and Opponent Data</param>
        public void FindMatch(PlayerMatchmakerData playerData, Action<PlayerMatchmakerData> onFinish)
        {
            UnityEngine.Debug.Log("PVP Call Number 6");

            if (loadingPlayer)
            {
                UnityEngine.Debug.LogWarning("Currently loading player! Cannot find match.");
            }
            UnityEngine.Debug.Log("PlayerStartTier" + matchmakerSearchParameters.PlayerRankOnInitialization);
            if (playerData == null)
            {
                if (GameDataManager.Instance.playerData != null)
                {
                    playerData = GameDataManager.Instance.playerData;
                }
                else
                {
                    GameDataManager.Instance.playerData = new PlayerMatchmakerData(PlayFabManager.Instance.PlayFabUserId, PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername, PlayFabManager.Instance.currentMember.MemberPersonalData.Level, 1, 1, PlayFabManager.Instance.avatarUrl.ToString());
                    playerData = GameDataManager.Instance.playerData;
                }
            }
            UnityEngine.Debug.Log("MMR TEST" + playerData.mmr);
            bool differentRank = matchmakerSearchParameters.PlayerRankOnInitialization != matchmakerSearchParameters.GetTier(playerData.mmr);
            this.playerData = playerData;
            if (onFoundMatch == null)
                onFoundMatch += onFinish;
            if (isInit == false || differentRank)
            {
                //Initialize the different categories of leaderboards
                matchmakerSearchParameters.Initialize(playerData, matchmakingLeaderboardEntries);
                //point to the player's tier initially
                matchmakerSearchParameters.currentSearchTier = matchmakerSearchParameters.GetTier(playerData.mmr);
            }
            isInit = true;
            RequestLeaderboard();
        }

        public void TestGet()
        {
            PlayerProfileViewConstraints constraint = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true,
                ShowStatistics = true,
                ShowLinkedAccounts = true,
            };

            //TODO: CHANGE THIS IF YOU HAVE A DIFFERENT IMPLEMENTATION OF LEADERBOARDS!
            LeaderboardUtils.RequestLeaderboard("ArenaMMR_Rank_0", 0, 100, constraint, OnSuccessfulRequest, OnFailedRequest);
        }
        [SerializeField]
        List<PlayerLeaderboardEntry> playersInCurrentTier = new List<PlayerLeaderboardEntry>();
        public bool test;
        public void StartFindRoutine()
        {

            PlayersScoreController.Instance.LevelGenerationBasedOnLevelsStackVSAI();
            if (findPlayerRoutine != null)
            {
                StopCoroutine(findPlayerRoutine);
                findPlayerRoutine = null;
            }

            findPlayerRoutine = StartCoroutine(FindPlayerRoutine());
        }

        private IEnumerator FindPlayerRoutine()
        {

            continueIndex = -1;
            UnityEngine.Debug.Log("Coming Here To Search");
            if (FoundMatch)
                matchmakerSearchParameters.searchParameterChangeCounter = 0;

            if (matchmakerSearchParameters.searchParameterChangeCounter > 2)
            {
                matchmakerSearchParameters.ResetSearchTier(playerData.mmr);
                matchmakerSearchParameters.searchParameterChangeCounter = 0;

                UnityEngine.Debug.Log("QQQQ MATCHMAKER QQQQ FAILED TO FIND AN OPPONENT. TRY AGAIN LATERz");

                InvokeFoundMatch(null);

                FoundMatch = true;
                yield break;
            }

            FoundMatch = false;

            string playerPlayfabId = playerData.playFabId;

            playersInCurrentTier = matchmakingLeaderboardEntries[matchmakerSearchParameters.currentSearchTier].LeaderboardEntries;

            if (playersInCurrentTier != null)
            {

                while (!FoundMatch)
                {

                    yield return new WaitForSeconds(0.25f);
                    if (encounteredValidIDs.Count == playersInCurrentTier.Count - 1)
                    {
                        encounteredValidIDs.Clear();
                        StartFindRoutine();
                        yield break;

                    }
                    UnityEngine.Debug.Log("Come Here Ever");
                    if (playersInCurrentTier == null)
                    {
                        matchmakerSearchParameters.OnPlayerNullOrEmpty();
                        playersInCurrentTier = GetPlayersInTier(matchmakerSearchParameters.currentSearchTier);
                    }
                    // int i = UnityEngine.Random.Range(0,playersInCurrentTier.Count);





                    if (continueIndex >= 0 && continueIndex < playersInCurrentTier.Count)
                    {
                        // i = continueIndex;
                        continueIndex = -1;
                    }

                    PlayerLeaderboardEntry opponentEntry = GetPlayerBasedOnSearch(playersInCurrentTier, playerData.mmr);
                    /* if (hasPreviousRecord)
                     {
                         for (int j = 0; j < playersInCurrentTier.Count; j++)
                         {
                             PlayerLeaderboardEntry currentEntry = playersInCurrentTier[j];
                             if (currentEntry.PlayFabId == LastOpponentPlayfabId)
                             {

                                 opponentEntry = currentEntry;
                                 continueIndex = j;
                             }
                         }
                     }
*/
                    if (FoundMatch)
                    {
                        //  continueIndex = i + 1;
                        encounteredValidIDs.Clear();
                        LastOpponentPlayfabId = "";

                        if (continueIndex >= playersInCurrentTier.Count)
                        {
                            playersInCurrentTier.Reverse();
                            continueIndex = 0;
                        }
                        loadingPlayer = false;
                        yield break;
                    }

                    if ((opponentEntry.PlayFabId != playerData.playFabId && encounteredValidIDs.Contains(opponentEntry.PlayFabId) == false))
                    {
                        loadingPlayer = true;
                        matchmakerSearchParameters.OnPlayerFound(opponentEntry, hasPreviousRecord, (playerValid, opponentMatchmakerData) =>
                        {
                            loadingPlayer = false;
                            if (FoundMatch)
                            {
                                return;
                            }

                            if (opponentMatchmakerData == null)
                            {
                                // continueIndex = i + 1;
                                LastOpponentPlayfabId = "";
                                return;
                            }

                            if (playerValid)
                            {
                                encounteredValidIDs.Add(opponentEntry.PlayFabId);
                                // continueIndex = i;
                                FoundMatch = true;
                                InvokeFoundMatch(opponentMatchmakerData);
                            }
                        });
                    }

                    if (DoSearchAgain)
                        ChangeSearchParameters(playersInCurrentTier);
                }
            }
            else
            {
                if (DoSearchAgain)
                    ChangeSearchParameters(playersInCurrentTier);
            }

            yield return null;
        }

        public void OnMatchEnd()
        {
            FoundMatch = false;
            //AiModeManager.Instance.AIFoundDifferences.Clear(); // clear any data of previous match

        }

        public void OnMatchFoundFromPVP(PlayerMatchmakerData opponentMatchmakerData)
        {


            UnityEngine.Debug.LogError("Enemy Data IS " + JsonUtility.ToJson(opponentMatchmakerData));
          
            UnityEngine.Debug.Log("Coming Here At All Or Not");
            loadingPlayer = false;
            if (FoundMatch)
            {
                UnityEngine.Debug.Log("Coming Here At All Or Not 1");
                return;
            }
            if (opponentMatchmakerData == null)
            {
                UnityEngine.Debug.Log("Coming Here At All Or Not 2");
                // continueIndex = i + 1;
                LastOpponentPlayfabId = "";
                return;
            }
            UnityEngine.Debug.Log("Coming Here At All Or Not 3");
            //   encounteredValidIDs.Add(opponentMatchmakerData.playFabId);
            // continueIndex = i;
            FoundMatch = true;
            InvokeFoundMatch(opponentMatchmakerData);
        }

        public PlayerLeaderboardEntry GetPlayerBasedOnSearch(List<PlayerLeaderboardEntry> allPlayers, int playerMMR)
        {

            int randomPlayer = UnityEngine.Random.Range(0, allPlayers.Count);
            return allPlayers[randomPlayer];
            //   return allPlayers.Find((ob)=>ob.PlayFabId== "E665CB0A740C950B"); for testing 
        }



        public bool DoSearchAgain;
        private void ChangeSearchParameters(List<PlayerLeaderboardEntry> playersInCurrentTier)
        {

            matchmakerSearchParameters.ChangeSearchParameters(playersInCurrentTier);

            //playersInCurrentTier = GetPlayersInTier(matchmakerSearchParameters.currentSearchTier);
            if (playersInCurrentTier != null)
            {
                encounteredValidIDs.Clear();
            }

            continueIndex = -1;
            RequestLeaderboard();
            //StartFindRoutine();
        }
        public bool notStuck=false;
        private void InvokeFoundMatch(PlayerMatchmakerData opponentMatchmakerData)
        {

            notStuck=true;
            if (onFoundMatch != null)
            {
                UnityEngine.Debug.LogWarning("Found match On PVP! 123");
                onFoundMatch(opponentMatchmakerData);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Found match event is empty! 456");
            }

            matchmakerSearchParameters.ResetSearchParameters();
        }

        //NOTE: for debug purpose only since a leaderboard system should be separate from the matchmaking system
        List<PlayerLeaderboardEntry> masterListCache;
        bool masterListInit = false;
        /// <summary>
        /// FOR DEBUG PURPOSES! You should have your own Player data fetcher when requesting additional data for revenge or any other player-related features
        /// </summary>
        /// <returns></returns>
        public void GetPlayerData(string playfabId, Action<PlayerMatchmakerData> onMatchMakerDataFetched)
        {
            if (masterListInit == false)
            {
                masterListCache = new List<PlayerLeaderboardEntry>();
                RequestMasterLeaderboard((masterList) =>
                {
                    masterListInit = true;
                    masterListCache.AddRange(masterList);
                    OnSuccessMasterFetch(playfabId, masterList, onMatchMakerDataFetched);
                }
                );
            }
            else
            {
                OnSuccessMasterFetch(playfabId, masterListCache, onMatchMakerDataFetched);
            }
        }
        /// <summary>
        /// Get the requested data from the master list on success
        /// </summary>
        /// <param name="playfabId"></param>
        /// <param name="masterList"></param>
        /// <param name="onMatchMakerDataFetched"></param>
        private void OnSuccessMasterFetch(string playfabId, List<PlayerLeaderboardEntry> masterList, Action<PlayerMatchmakerData> onMatchMakerDataFetched)
        {
            for (int j = 0; j < masterList.Count; j++)
            {
                PlayerLeaderboardEntry currentEntry = masterList[j];
                if (currentEntry.PlayFabId == playfabId)
                {
                    PlayerMatchmakerData matchMakerData = new PlayerMatchmakerData(playfabId, currentEntry.DisplayName, currentEntry.StatValue, currentEntry.Position, 1, currentEntry.Profile.AvatarUrl != null ? currentEntry.Profile.AvatarUrl : ""); //insert your level fetching method on the last parameter
                    PlayerMatchamkingPartyData partyData = new PlayerMatchamkingPartyData() { partyAttackLevel = UnityEngine.Random.Range(0, 200) };
                    matchMakerData.partyData = partyData;

                    onMatchMakerDataFetched(matchMakerData);
                    return;
                }
            }

            onMatchMakerDataFetched(null);
            UnityEngine.Debug.LogError("data does not exist in leaderboard! " + playfabId);
        }

        #region Leaderboard Requesting

        private void RequestMasterLeaderboard(Action<List<PlayerLeaderboardEntry>> onFinish)
        {
            PlayerProfileViewConstraints constraint = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true,
                ShowStatistics = true,
                ShowLinkedAccounts = true,
            };

            LeaderboardUtils.RequestLeaderboard(masterLeaderboardName, 0, 100, constraint, onFinish, OnFailedRequest);
        }

        private void RequestLeaderboard()
        {
            UnityEngine.Debug.Log("Current Leaderboard Call 1" + matchmakerSearchParameters.currentSearchTier);
            PlayerProfileViewConstraints constraint = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true,
                ShowStatistics = true,
                ShowLinkedAccounts = true,
            };

            loadingPlayer = true;
            //TODO: CHANGE THIS IF YOU HAVE A DIFFERENT IMPLEMENTATION OF LEADERBOARDS!            
            UnityEngine.Debug.Log("doing a request! Name:" + CurrentLeaderboardName);
            if (GameDataManager.Instance.playerData != null)
            {
                LeaderboardUtils.RequestLeaderboard(CurrentLeaderboardName
                    , 0, 100, constraint, OnSuccessfulRequest, OnFailedRequest);
            }
            else
            {
                LeaderboardUtils.RequestLeaderboard(CurrentLeaderboardName, 0, 100, constraint, OnSuccessfulRequest, OnFailedRequest);
            }
        }
        private void OnSuccessfulRequest(List<PlayerLeaderboardEntry> entries)
        {


            notStuck=false;
            matchmakingLeaderboardEntries[matchmakerSearchParameters.currentSearchTier].LeaderboardEntries = entries;
            loadingPlayer = false;
            if (PhotonManager.Instance.UsePVP)
            {
                PhotonManager.Instance.JoinRandomRoom();
            }
            else
            {

                StartFindRoutine();

            }
        }

        private void OnFailedRequest()
        {
            loadingPlayer = false;
            InvokeFoundMatch(null);
        }

        public void ClearListeners()
        {
            onFoundMatch = null;
        }

        #endregion       
    }
}
