using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using System.Linq;
using System;
namespace BGGamesCore.Matchmaker
{
    [CreateAssetMenu(fileName = "ArenaMatchmakingSearchData", menuName = "Matchmaking/Data/Create Arena Data")]
    public class ArenaMatchmakingSearchParameter : AMatchmakerSearchParameter
    {
        public enum ArenaSearchModes
        {
            OpposingPartyDamageLower = 0,
            OpposingPartyDamageHigher
        }
        //Feel free to move this somewhere else such as your leaderboard implementation.
        public enum ArenaTiers
        {
            UNRANKED = 0,
            BRONZE,
            SILVER,
            GOLD,
            PLATINUM,
            CHAMPION
        }

        private void Awake()
        {
            leaderboardNames = new string[]
            {
                "ArenaMMR_Rank_0",
                "ArenaMMR_Rank_1",
                "ArenaMMR_Rank_2",
                "ArenaMMR_Rank_3",
                "ArenaMMR_Rank_4",
                "ArenaMMR_Rank_5",
            };

            SearchEnumSize = ArrayUtils.GetEnumValues<ArenaSearchModes>().Count();
        }


        public List<MmrRanges> MmrRanges = new List<MmrRanges>();
        public override MmrRanges GetMmrRange(string ArenaName) 
        {

            return MmrRanges.Find((ob)=>ob.MmrArena==ArenaName);

        }
        public override string GetTierString(int tier)
        {
            ArenaTiers toReturn = (ArenaTiers)tier;
            return toReturn.ToString();
        }
        public override string GetPlayerCurrentLeaderBoardName(int mmr) 
        {
           return leaderboardNames[GetTier(mmr)];        
        }
        public override int GetTier(int mmr)
        {
            if (mmr >= 150 && mmr < 399)
            {
                return 1;
            }
            else if (mmr >= 400 && mmr < 699)
            {
                return 2;
            }
            else if (mmr >= 700 && mmr < 999)
            {
                return 3;
            }
            else if (mmr >= 1000 && mmr < 1299)
            {
                return 4;
            }
            else if (mmr >= 1300)
            {
                return 5;
            }
            else
                return 0;

        }

        public override void ChangeSearchParameters(List<PlayerLeaderboardEntry> playersInCurrentRank)
        {
            if (currentSearchMode < SearchEnumSize)
            {
                if (MatchmakingSystem.Instance.FoundMatch == false)
                {
                    currentSearchMode++;
                    return;
                }
            }

            currentSearchMode = (int)ArenaSearchModes.OpposingPartyDamageLower;
            searchParameterChangeCounter++;

            if (searchRankIncremented)
            {
                if(currentSearchTier != (int)ArenaTiers.UNRANKED)
                {
                    currentSearchTier = GetTier(playerData.mmr);
                    --currentSearchTier;
                    searchRankIncremented = true;
                }
                else
                {
                    searchRankIncremented = false;
                }
            }
            else
            {
                if(currentSearchTier != (int)ArenaTiers.CHAMPION)
                {
                    ++currentSearchTier;
                    searchRankIncremented = true;
                }
                else
                {
                    searchRankIncremented = false;
                }
            }
        }

        public override void OnPlayerNullOrEmpty()
        {
            currentSearchTier--;
            currentSearchMode = (int)ArenaSearchModes.OpposingPartyDamageLower;
        }
    
        public override void OnPlayerFound(PlayerLeaderboardEntry opponentLeaderboardEntry, bool previousPlayer, Action<bool, PlayerMatchmakerData> onFinished)
        {
            string opponentPlayfabId = opponentLeaderboardEntry.PlayFabId;

            //Do data fetching here

            //SAMPLE INSTANTIATION FOR OPPONENT PARTY DATA
            PlayerMatchmakerData matchMakerData = new PlayerMatchmakerData(opponentLeaderboardEntry.PlayFabId, opponentLeaderboardEntry.DisplayName, opponentLeaderboardEntry.StatValue, opponentLeaderboardEntry.Position, 1,opponentLeaderboardEntry.Profile.AvatarUrl!=null?opponentLeaderboardEntry.Profile.AvatarUrl:""    ); //insert your level fetching method on the last parameter
            PlayerMatchamkingPartyData partyData = new PlayerMatchamkingPartyData() { partyAttackLevel = UnityEngine.Random.Range(0, 200) };
            matchMakerData.partyData = partyData;

            //by-pass checks if previous encountered player
            if (previousPlayer)
                onFinished(true, matchMakerData);
            else
                OnPartyLevelLoaded(matchMakerData, onFinished);
        }

        private void OnPartyLevelLoaded(PlayerMatchmakerData opponentMatchmakerData, Action<bool, PlayerMatchmakerData> onFinished)
        {
            ArenaSearchModes searchMode = (ArenaSearchModes)currentSearchMode;
            switch (searchMode)
            {
                case ArenaSearchModes.OpposingPartyDamageLower:
                    this.DamageEnemyLowerCheck(opponentMatchmakerData, onFinished);
                    break;
                case ArenaSearchModes.OpposingPartyDamageHigher:
                    this.DamageEnemyHigherCheck(opponentMatchmakerData, onFinished);
                    break;
                default:
                    onFinished(false, null);
                    break;
            }
        }
        #region Damage checks
        private void DamageEnemyLowerCheck(PlayerMatchmakerData opponentMatchmakerData, Action<bool, PlayerMatchmakerData> onFinished)
        {
            onFinished(true, opponentMatchmakerData);
        }

        private void DamageEnemyHigherCheck(PlayerMatchmakerData opponentMatchmakerData, Action<bool, PlayerMatchmakerData> onFinished)
        {
            onFinished(true, opponentMatchmakerData);
        }

        #endregion
    }
}
