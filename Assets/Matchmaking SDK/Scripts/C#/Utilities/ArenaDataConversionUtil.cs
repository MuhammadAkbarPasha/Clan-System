using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace BGGamesCore.Matchmaker.Utils
{
    public class ArenaDataConversionUtil
    {
        public static void GetUserMatchmakingData(string playFabId, Action<PlayerMatchmakerData> OnReceivedMatchmakerDetails, Action OnError)
        {
            ArenaUtilities.GetUserArenaDetails(playFabId, (arenaData)=>
            {
                PlayerMatchmakerData matchmakerData = new PlayerMatchmakerData(arenaData.playFabId, arenaData.name, arenaData.mmrWeekly, arenaData.position, arenaData.level, arenaData.avatarUrl != null ? arenaData.avatarUrl : "");
                OnReceivedMatchmakerDetails(matchmakerData);
            }, OnError);
        }
    }
}