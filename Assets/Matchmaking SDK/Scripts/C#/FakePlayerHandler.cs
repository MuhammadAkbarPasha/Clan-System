using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BGGamesCore.Matchmaker;
using PlayFab.ClientModels;
//using WordConnect;
/// <summary>
/// Forces the game to use a specific player from your leaderboard
/// </summary>
public class FakePlayerHandler : MonoBehaviour
{
    public static FakePlayerHandler Instance { get; private set; }

    /// <summary>
    /// Player Id that will be fetched once the game starts.
    /// </summary>
    [SerializeField] private string playerId;

    /// <summary>
    /// Data that will be used in the examples.
    /// </summary>
    public PlayerMatchmakerData PlayerData
    {
        get; private set;
    }

    /// <summary>
    /// Playfab initializer that's attached to the object.
    /// </summary>
    private PlayfabBootstrapper bootstrapper;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            bootstrapper = GetComponent<PlayfabBootstrapper>();
        }
    }
    public string GetFakePlayerId() 
    {
        return playerId;
    }


    public IEnumerator GetLeaderboard() 
    {

        playerId=   PlayFabManager.Instance.PlayFabUserId;
        yield return new WaitForSeconds(2f);
        PlayerProfileViewConstraints constraint = new PlayerProfileViewConstraints
        {
            ShowDisplayName = true,
            ShowAvatarUrl = true,
            ShowStatistics = true,
            ShowLinkedAccounts = true,
        };

        BGGamesCore.Matchmaker.Utils.LeaderboardUtils.RequestLeaderboard(MatchmakingSystem.Instance.masterLeaderboardName, 0, 100, constraint, (entries) =>
        {
            foreach (PlayerLeaderboardEntry entry in entries)
            {
                if (entry.PlayFabId == PlayFabManager.Instance.PlayFabUserId)
                {
                    PlayerData = new PlayerMatchmakerData(entry.PlayFabId, entry.DisplayName, entry.StatValue, entry.Position, 1, entry.Profile.AvatarUrl != null ? entry.Profile.AvatarUrl : "");
                    GameDataManager.Instance.playerData= new PlayerMatchmakerData(entry.PlayFabId, entry.DisplayName, entry.StatValue, entry.Position, 1, entry.Profile.AvatarUrl != null ? entry.Profile.AvatarUrl : "");

                    PlayerMatchamkingPartyData partyData = new PlayerMatchamkingPartyData() { partyAttackLevel = UnityEngine.Random.Range(0, 200) };
                    PlayerData.partyData = partyData;
                    PlayerData.playerCurrentLeaderboard = MatchmakingSystem.Instance.GetSearchParameters.GetPlayerCurrentLeaderBoardName(entry.StatValue);
                    Debug.Log("Found your player! ID:" + playerId + " Username:" + entry.DisplayName);
                    Debug.Log(PlayerData.level);
                    UIBattleLobby.Instance.Init();
                    //UIController.Instance.SetMmrOnHome(PlayerData.mmr);
                    return;
                }
            }
        });

        
    }
}
