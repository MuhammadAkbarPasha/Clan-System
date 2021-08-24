using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BGGamesCore.Matchmaker
{
    [System.Serializable]
    public class PlayerMatchmakerData
    {
        public string avatarUrl;
        public string playFabId;
        public string name;
        public int mmr;
        public int mmrMonthly;
        public int position;
        public int winStreak;
        public int level;
        public string playerCurrentLeaderboard;
        public PlayerMatchamkingPartyData partyData;

        public PlayerMatchmakerData(PlayerMatchmakerData PlayerData) 
        {
            this.playFabId = PlayerData.playFabId;
            this.name = PlayerData.name;
            this.mmr = PlayerData.mmr;
            this.position = PlayerData.position;
            this.level = PlayerData.level;
            this.partyData = PlayerData.partyData;
            this.avatarUrl = PlayerData.avatarUrl;
        }

        public PlayerMatchmakerData(string playFabId, string name, int mmr, int position, int level,string avatarUrl)
        {
            this.playFabId = playFabId;
            this.name = name;
            this.mmr = mmr;
            this.position = position;
            this.level = level;
            this.avatarUrl = avatarUrl;
        }

        public void ClearData()
        {
            playFabId = "";
            name = "";
            mmr = -1;
            mmrMonthly = -1;
            position = -1;
            winStreak = 0;
            level = 0;
        }
        public PlayerMatchmakerData()
        {
        }
    }

}