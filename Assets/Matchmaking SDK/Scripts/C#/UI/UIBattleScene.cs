using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGamesCore.Matchmaker.Utils;
using BGGamesCore.Matchmaker;
//using WordConnect;

public class UIBattleScene : MonoBehaviour
{
    [SerializeField] UIPlayerInfoCard playerInfo;
    [SerializeField] UIPlayerInfoCard enemyInfo;

    /// <summary>
    /// Unfiltered leaderboard
    /// </summary>
    private string masterLeaderboardName = "";
    /// <summary>
    /// Current category/rank leaderboard the player is in
    /// </summary>
    private string categoryLeaderboardName = "";
    public void Init()
    {
        masterLeaderboardName = MatchmakingSystem.Instance.masterLeaderboardName;
        categoryLeaderboardName = MatchmakingSystem.Instance.CurrentLeaderboardName;
        Debug.Log("Current Leaderboard"+ categoryLeaderboardName);
        SetupPlayerViews();
    }
   
    private void SetupPlayerViews()
    {
        playerInfo.Initialize(GameDataManager.Instance.playerData.name);
        enemyInfo.Initialize(GameDataManager.Instance.opponentData.name);
       

    }

    private void HidePlayerViews()
    {
        playerInfo.ToggleView(false);
        enemyInfo.ToggleView(false);
    }

    #region Button actions
    /// <summary>
    /// Simulates Regular Win
    /// </summary>
    public void OnRegularWinTapped()
    {
       MatchmakerUtils.GetMatchData("Win",PlayFabManager.Instance.playerCountry.CountryLevel, MatchmakingSystem.Instance.masterLeaderboardName, GameDataManager.Instance.playerData.playFabId, (matchData) =>
        {
            GameDataManager.Instance.playerData.mmr = matchData.MasterLeaderBoardMMR;
            UIBattleLobby.Instance.playerView.Initialize(GameDataManager.Instance.playerData,true);
            Debug.Log("Player won!");
            MatchmakingSystem.Instance.playerData = GameDataManager.Instance.playerData;
            UIBattleLobby.Instance.playerData = GameDataManager.Instance.playerData;
            //UIController.Instance.SetMmrOnHome(matchData.MasterLeaderBoardMMR);
        }, null);
    }

    /// <summary>
    /// Simulates Regular Lose
    /// </summary>
    public void OnRegularLoseTapped()
    {
        MatchmakerUtils.GetMatchData("Lose", PlayFabManager.Instance.playerCountry.CountryLevel, MatchmakingSystem.Instance.masterLeaderboardName, GameDataManager.Instance.playerData.playFabId, (matchData) =>
        {
            GameDataManager.Instance.playerData.mmr = matchData.MasterLeaderBoardMMR;
            UIBattleLobby.Instance.playerView.Initialize(GameDataManager.Instance.playerData,true);
            Debug.Log("Player won!");
            MatchmakingSystem.Instance.playerData = GameDataManager.Instance.playerData;
            UIBattleLobby.Instance.playerData = GameDataManager.Instance.playerData;
        }, null);
    }
    /// <summary>
    /// Adds the PlayFabId of the Player to the Opponent
    /// </summary>
    public void OnAddPlayerToOpponentRevenge()
    {
        string playerId = GameDataManager.Instance.playerData.playFabId;
        string opponentId = GameDataManager.Instance.opponentData.playFabId;
        RevengeUtils.UpdateRevengeList(playerId, opponentId, true, ()=> {
            Debug.Log("Player was added to opponent revenge list!");
        }, null);
    }

    /// <summary>
    /// Adds the PlayFabId of the Opponent to the Player
    /// </summary>
    public void OnAddOpponentToPlayerRevenge()
    {
        string playerId = GameDataManager.Instance.playerData.playFabId;
        string opponentId = GameDataManager.Instance.opponentData.playFabId;
        RevengeUtils.UpdateRevengeList(playerId, opponentId, false, () => {
            Debug.Log("Opponent was added to player revenge list!");
        }, null);
    }
    #endregion
}
