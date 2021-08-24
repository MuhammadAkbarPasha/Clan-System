using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BGGamesCore.Matchmaker;
using BGGamesCore.Matchmaker.Utils;
using Chamoji.Social;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;
//using WordConnect;
using System;

public class UIBattleLobby : MonoBehaviour
{
    //Sample views
    public UIPlayerInfoCard playerView;
    [SerializeField] private UIPlayerInfoCard opponentView;
    [SerializeField] private PlayerInGameCard inGamePlayerView;
    [SerializeField] private PlayerInGameCard inGameOpponentView;

    bool isDestroyed;
    public PlayerMatchmakerData playerData;
    public PlayerMatchmakerData opponentData;
    public static UIBattleLobby Instance;
    public float MatchStartDelay = 0f;
    //   public MatchingScreenManager myMatchingScript;



    [SerializeField]
    Sprite PlayerSprite;
    [SerializeField]
    Sprite EnemySprite;

    public void SetPlayerSprite(Sprite sprite)
    {
        PlayerSprite = sprite;
    }
    public void SetEnemySprite(Sprite sprite)
    {
        EnemySprite = sprite;
    }


    public Sprite GetPlayerSprite()
    {
        return PlayerSprite;
    }
    public Sprite GetEnemySprite()
    {
        return EnemySprite;
    }



    /// <summary>
    /// Data fetched from the backend. Used for tracking the last opponent fought by the player and other entry requirements in the PVP system.
    /// </summary>
    private PvPUserData pvpUserData;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Init()
    {
        StartCoroutine(WaitForData());
    }

    private IEnumerator WaitForData()
    {
        while (PlayFabManager.Instance.PlayerAvatar == null && PlayFabManager.Instance.avatarUrl.Length > 1)
        {
            yield return new WaitForSeconds(.25f);
        }

        playerData = FakePlayerHandler.Instance.PlayerData;
        Debug.LogError("Umesh player data : " + playerData);
        playerView.Initialize(playerData, true);
        isDestroyed = false;
    }

    private void CheckForPreviousEnemyID()
    {
        MatchmakerUtils.GetPVPData((data) =>
        {
            if (isDestroyed) return;

            pvpUserData = data;
            if (GameDataManager.Instance.isDoingRevenge)
                MatchmakingSystem.Instance.LastOpponentPlayfabId = GameDataManager.Instance.revengeId;
            else
                MatchmakingSystem.Instance.LastOpponentPlayfabId = data.lastOpponentPlayfabID;

            BeginMatchFind();
        }, null);
    }

    private void OnDestroy()
    {
        isDestroyed = true;
        MatchmakingSystem.Instance.ClearListeners();
        playerData = null;
        opponentData = null;
    }
    /// <summary>
    /// 
    /// </summary>
    private void BeginMatchFind()
    {
        isDestroyed = false;
        MatchmakingSystem.Instance.FindMatch(playerData, OnOpponentFound);
    }

    /// <summary>
    /// Attach the enemy
    /// </summary>
    /// <param name="opponentData"></param>
    private void OnOpponentFound(PlayerMatchmakerData opponentData)
    {
        //Prevent errors when going back to sample scenes and the request finishes.
        if (isDestroyed)
        {
            Destroy(this);
            return;
        }

        if (opponentData != null)
        {
            MatchmakingSystem.Instance.loadingPlayer = true;
            pvpUserData.lastOpponentPlayfabID = opponentData.playFabId;

            MatchmakerUtils.SetPvPData(pvpUserData, () =>
            {
                MatchmakingSystem.Instance.loadingPlayer = false;
                Debug.LogError("url : " + opponentData.avatarUrl + "   name : " + opponentData.name);
                //Debug.LogError("Umesh player data : " + opponentData);
                StartCoroutine(LoadOpponentImage(opponentData));
                if (opponentView.isActiveAndEnabled)
                {
                    opponentView.Initialize(opponentData);
                }
                this.opponentData = opponentData;
                StartCoroutine(GameInitializeAfterDelay());
            }, null);

        }
        else
        {
            //Put your popup here.
            Debug.LogWarning("Could not find an enemy!");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="myOpponent"></param>
    /// <returns></returns>
    IEnumerator LoadOpponentImage(PlayerMatchmakerData myOpponent)
    {
        yield return new WaitForSeconds(0);
        Sprite s = null;
        if (Uri.IsWellFormedUriString(myOpponent.avatarUrl, UriKind.Absolute))
        {

            Texture2D t;
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(myOpponent.avatarUrl);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                t = ((DownloadHandlerTexture)request.downloadHandler).texture;
                s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0), .01f);
            }
        }
    }
    /// <summary>
    /// Attach the enemy
    /// </summary>
    /// <param name="opponentData"></param>
    private void OnAIFound(PlayerMatchmakerData opponentData)
    {
        //Prevent errors when going back to sample scenes and the request finishes.
        if (isDestroyed)
        {
            Destroy(this);
            return;
        }

        if (opponentData != null)
        {
            MatchmakingSystem.Instance.loadingPlayer = true;
            MatchmakingSystem.Instance.loadingPlayer = false;
            opponentView.Initialize(opponentData);
            Debug.LogError("url : " + opponentData.avatarUrl + "   name : " + opponentData.name);
            this.opponentData = opponentData;
            StartCoroutine(GameInitializeAfterDelay());
        }
        else
        {
            //Put your popup here.
            Debug.LogWarning("Could not find an enemy!");
        }
    }


    public IEnumerator FakePlayerWait()
    {
        opponentView.ToggleView(false);
        yield return new WaitForSeconds(4f);
        // OnAIFound(new PlayerMatchmakerData("123","AI",9999,1,1));
    }

    public IEnumerator GameInitializeAfterDelay()
    {

        yield return new WaitForSeconds(MatchStartDelay);
        PopulateInGameUI(playerData, opponentData);
        GameDataManager.Instance.playerData = new PlayerMatchmakerData(playerData);
        GameDataManager.Instance.opponentData = new PlayerMatchmakerData(opponentData);

    }
    public void PopulateInGameUI(PlayerMatchmakerData playerData, PlayerMatchmakerData opponentData)
    {
        PopulateIndividualCard(inGamePlayerView, playerData);
        PopulateIndividualCard(inGameOpponentView, opponentData);


    }

    public void PopulateIndividualCard(PlayerInGameCard playerDataContainer, PlayerMatchmakerData Data)
    {
        playerDataContainer.SetData(Data.name);

    }
    public IEnumerator Search()
    {
        yield return null;
        CheckForInternet();
        GameDataManager.Instance.ClearRevenge();
        opponentView.ToggleView(false);
        CheckForPreviousEnemyID();
    }

    public void Battle()
    {
        GameDataManager.Instance.playerData = new PlayerMatchmakerData(playerData);
        GameDataManager.Instance.opponentData = new PlayerMatchmakerData(opponentData);
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleBattleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void CheckForInternet()
    {
        //Check Internet Connection Here
    }
}
