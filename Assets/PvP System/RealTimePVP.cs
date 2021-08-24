using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using System;
using BGGamesCore.Matchmaker;
using System.Linq;

public class RealTimePVP : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    [Header("Is This Instance Player1 Or Player2 ")]
    public PVPEnums.PlayerCurrentState playerCurrentState = PVPEnums.PlayerCurrentState.None;

    [Header("Player Match Making Data")]
    public PlayerMatchmakerData PlayerDataForSyncing = new PlayerMatchmakerData();
    [Header("Multiplayer Manager Class Reference")]
    public PlayersScoreController playersScoreController;
    [Header("Photon View Of This Instance")]
    [SerializeField]
    public PhotonView pv;
    [Header("Player Properties")]
    public List<int> PlayerFoundDifferences = new List<int>();
    public StackClass CurrentCityStack = new StackClass();
    public StackSystem CurrentGeneralStack = new StackSystem();
    private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] double serverWinTime = double.MaxValue;
    public double GetServerTime()
    {
        return serverWinTime;
    }
    public void SetServerTime()
    {
        serverWinTime = PhotonNetwork.Time;
    }
    /// <summary>
    /// Update the list of found differences on the remote instance.
    /// </summary>
    /// <param name="foundDifferencesList"></param>
    public void FindDifferencesOnOtherPlayer(List<int> foundDifferencesList)
    {
        ArenaAndMatchMakingBridge.Instance.DebugText.text = "\n";
        foreach (int difference in foundDifferencesList)
        {
            if (!PlayerFoundDifferences.Contains(difference))
            {
                if (!pv.Owner.IsLocal)
                {
                    // Add difference on Local player too (Could be AI) 
                    //  AiModeManager.Instance.AiFindDifference(difference);
                    ArenaAndMatchMakingBridge.Instance.DebugText.text += pv.IsMine + " Difference " + difference;
                    PlayerFoundDifferences.Add(difference);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (playersScoreController == null && playerCurrentState == PVPEnums.PlayerCurrentState.None)
        {
            OnStartCalls();
        }
    }


    /// <summary>
    /// Call the function to set player state and start a coroutine to connect with player score controller.
    /// </summary>
    private void OnStartCalls()
    {
        serverWinTime = double.MaxValue;
        SetupPlayerState();
        StartCoroutine(ConnectWithPlayersScoreController());
    }


    /// <summary>
    /// Set player avatar URL to player profile index current URL is not appropriate
    /// Update player data for syncronization on both ends.
    /// </summary>
    public void GetPlayerDataForSyncingFromManager()
    {
        Debug.Log("Game Data Manager Here", GameDataManager.Instance.gameObject);
        if (photonView.Owner.IsLocal)
        {
            GameDataManager.Instance.UpdateData();
            // if (!Uri.IsWellFormedUriString(GameDataManager.Instance.playerData.avatarUrl, UriKind.Absolute))
            // {

            //     GameDataManager.Instance.playerData.avatarUrl = EditProfileScript.Instance.ProfileIndex.ToString();

            // }
            PlayerDataForSyncing = GameDataManager.Instance.playerData;
        }
    }


    /// <summary>
    /// Set the player state to player 1 if the player is master clint, otherwise it set to player 2.
    /// </summary>
    public void SetupPlayerState()
    {
        if (pv.Owner.IsMasterClient)
        {
            playerCurrentState = PVPEnums.PlayerCurrentState.Player1;
        }
        else
        {
            playerCurrentState = PVPEnums.PlayerCurrentState.Player2;
        }
    }


    /// <summary>
    /// A coroutine which wait for one second and then setup the PlayersScoreController.
    /// And then set the instance to current instance (this).
    /// </summary>
    /// <returns></returns>
    public IEnumerator ConnectWithPlayersScoreController()
    {
        yield return new WaitForSeconds(1);
        playersScoreController = FindObjectOfType<PlayersScoreController>();
        playersScoreController.SetupInstance(this);
    }



    /// <summary>
    /// It is a function to make RPC call to increase points.
    /// </summary>
    /// <param name="pointNumber"></param>
    public void IncreasePointsRPCCaller(int pointNumber)
    {
        // if (PlayersScoreController.Instance.GameStatus != PVPEnums.GameStatus.Playing)
        //     return;
        Debug.Log("Here To Increase Points " + pointNumber);
        if (pv.Owner.IsLocal)
        {
            PlayerFoundDifferences.Add(pointNumber);
        }
        SetCustomPropertiesForDifferencesFound();
        pv.RPC(nameof(IncreasePoints), RpcTarget.All, pointNumber);
    }


    /// <summary>
    /// This is a RPC function to insreace points by given point number.
    /// It is called on player local instance as well as remote instance.
    /// </summary>
    /// <param name="pointNumber"></param>
    [PunRPC]
    private void IncreasePoints(int pointNumber)
    {
        Debug.Log("RPC :" + photonView.Owner.IsLocal);
        // if (PlayersScoreController.Instance.GameStatus != PVPEnums.GameStatus.Playing)
        //     return;

        if (photonView.Owner.IsLocal)
        {
            // run for this player 
            return;
        }
        else
        {
            //Add difference on Local player too (Could be AI) 
            //AiModeManager.Instance.AiFindDifference(pointNumber);
            //PlayersScoreController.CheckAndAddDifference(this,pointNumber);
            // run for other player 
        }
    }


    /// <summary>
    /// Freeze local player
    /// </summary>
    [PunRPC]
    private void Freeze()
    {
        Debug.Log("RPC :" + photonView.Owner.IsLocal);

        if (photonView.Owner.IsLocal)
        {
            ArenaAndMatchMakingBridge.Instance.DebugText.text = "";
            ArenaAndMatchMakingBridge.Instance.PrintDebugOnNextLine("This Player Freeze Logic Here");
            Debug.Log("This Player Freeze Logic Here");
            FreezeMyPlayer();
            return;
        }
    }


    /// <summary>
    /// Set win time of the player
    /// </summary>
    [PunRPC]
    private void SetWinTime(double serverTime)
    {
        if (!photonView.Owner.IsLocal)
            serverWinTime = serverTime;
    }

    /// <summary>
    /// Start match on the local with the passed parameters 
    /// </summary>
    /// <param name="levelNumber">Level number to start</param>
    /// <param name="matchType">Match type or mode</param>
    /// <param name="stackName">Which stack to retrieve level from</param>
    [PunRPC]
    private void StartMatch(int levelNumber, string matchType, string stackName)
    {
        if (photonView.Owner.IsLocal)
        {
            ArenaAndMatchMakingBridge.Instance.PrintDebugOnNextLine("RPC Level Type " + matchType + "Level Number " + levelNumber + "Stack Name" + stackName);
            PlayersScoreController.Instance.levelObject = new LevelClass(levelNumber, matchType, stackName);
        }
    }




    /// <summary>
    /// This is a RPC function to show emote of a given emote number.
    /// It is called on player local instance as well as remote instance.
    /// </summary>
    /// <param name="emoteNumber"></param>
    [PunRPC]
    private void ShowEmote(int emoteNumber)
    {
        Debug.Log("RPC : " + nameof(ShowEmote));
        if (photonView.Owner.IsLocal)
        {
            // run for this player 
            return;
        }
        else
        {
            EmotesManager.Instance.ShowEmoteRemote(emoteNumber);
            // run for other player 
        }
    }


    /// <summary>
    /// This is a RPC function to show message.
    /// It is called on player local instance as well as remote instance.
    /// </summary>
    /// <param name="message"></param>
    [PunRPC]
    private void ShowMessage(string message)
    {
        Debug.Log("RPC : " + nameof(ShowMessage));
        if (photonView.Owner.IsLocal)
        {
            // run for this player 
            return;
        }
        else
        {
            EmotesManager.Instance.ShowEmoteRemote(message);
            // run for other player 
        }
    }

    /// <summary>
    /// Add freeze functionality here
    /// </summary>    
    void FreezeMyPlayer()
    {
        //  AiModeManager.Instance.FreezeMyPlayer();
    }


    /// <summary>
    /// Function to call Freeze rpc
    /// </summary>
    public void FreezeRPCCaller()
    {
        pv.RPC(nameof(Freeze), RpcTarget.All);
    }


    /// <summary>
    /// Function to call StartMatch rpc 
    /// </summary>
    /// <param name="levelNumber">Level number to start</param>
    /// <param name="matchType">Match type or mode</param>
    /// <param name="stackName">Which stack to retrieve level from</param>
    public void StartMatchRPCCaller(int levelNumber, string matchType, string stackName)
    {
        pv.RPC(nameof(StartMatch), RpcTarget.All, levelNumber, matchType, stackName);
    }


    /// <summary>
    /// Function to call SetWinTime RPC 
    /// </summary>
    public void SetWinTimeRPCCaller()
    {
        SetServerTime();
        pv.RPC(nameof(SetWinTime), RpcTarget.AllBuffered, serverWinTime);
    }


    /// <summary>
    /// Function to call ShowEmote RPC
    /// </summary>
    /// <param name="emoteNumber"></param>
    public void EmoteRPC(int emoteNumber)
    {
        Debug.Log("Emote Number Is: " + emoteNumber);
        pv.RPC(nameof(ShowEmote), RpcTarget.AllBuffered, emoteNumber);
    }


    /// <summary>
    /// Function to call ShowMessage RPC
    /// </summary>
    /// <param name="message"></param>
    public void MessageRPC(string message)
    {
        Debug.Log("Emote Number Is: " + message);
        pv.RPC(nameof(ShowMessage), RpcTarget.AllBuffered, message);
    }

    
    /// <summary> 
    /// A function to update player data on local instance if stream.IsWriting is true, otherwise update player data on remote instance.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(JsonUtility.ToJson(PlayerDataForSyncing));
        }
        else
        {
            PlayerDataForSyncing = JsonUtility.FromJson<PlayerMatchmakerData>((string)stream.ReceiveNext());
        }
    }


    /// <summary>
    /// Clear instance of this object 
    /// </summary>
    public void DestroyOnMatchEnd()
    {
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Make calls to update player data and properties when a player is instanciated.
    /// </summary>
    /// <param name="info"></param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\n Player Instantiated " + info.photonView.IsMine;
        OnStartCalls();
        GetPlayerDataForSyncingFromManager();
        CheckCustomPropertiesForDifferencesFound();
        CheckCustomPropertiesForStackSystem();

    }


    /// <summary>
    /// Clear custom properties for current player. Call this to clear properties after match end
    /// </summary>
    public void ClearCustomProperties()
    {
        if (photonView.Owner != null)
        {
            if (photonView.Owner.IsLocal)
                pv.Owner.CustomProperties.Clear();
        }
    }


    /// <summary>
    /// Update found differences list on local instance if player is local, otherwise make call to SetCustomPropertiesForDifferencesFound. 
    /// </summary>
    public void CheckCustomPropertiesForDifferencesFound()
    {
        if (pv.Owner.CustomProperties.ContainsKey("DifferencesFound"))
        {
            int[] bomb = (int[])pv.Owner.CustomProperties["DifferencesFound"];
            List<int> foundDifferencesList = new List<int>(bomb);
            FindDifferencesOnOtherPlayer(foundDifferencesList);
        }
        else
        {
            SetCustomPropertiesForDifferencesFound();
        }
    }


    /// <summary>
    /// Flow control function. If player is local it sets custom properties and player is not local
    /// then it receive custom properties 
    /// </summary>
    public void CheckCustomPropertiesForStackSystem()
    {
        if (pv.Owner.IsLocal)
        {
            SetCustomPropertiesForStackSystem();
        }
        else
        {
            StartCoroutine(GetCustomPropertiesForStackSystem());
        }
    }


    /// <summary>
    /// Update found differences list on remote instance.
    /// </summary>
    public void SetCustomPropertiesForDifferencesFound()
    {
        _myCustomProperties["DifferencesFound"] = PlayerFoundDifferences.ToArray();
        pv.Owner.SetCustomProperties(_myCustomProperties);
    }


    /// <summary>
    /// Setup custom properties of the player for stack system
    /// </summary>
    private void SetCustomPropertiesForStackSystem()
    {
        if (!pv.Owner.IsLocal)
            return;
        CurrentCityStack = PlayerLevelsDataManager.Instance.cityStackSystem.GetStack(PhotonManager.Instance.currentArena);
        CurrentCityStack.LevelsUnPlayed = CurrentCityStack.LevelsUnPlayed.Where(x => ArenaAndMatchMakingBridge.Instance.levelsToBeFiltered.LevelsToBeFiltered.All(y => y != x)).ToList();
        CurrentGeneralStack.stacks = PlayerLevelsDataManager.Instance.generalStackSystem.
        GetStack(ArenaAndMatchMakingBridge.Instance.
        GetGeneralList(CurrentCityStack.name));
        if (CurrentCityStack == null)
        {
            CurrentCityStack = new StackClass("London", "City");
        }
        _myCustomProperties["CurrentCityStack"] = JsonUtility.ToJson(CurrentCityStack);
        _myCustomProperties["CurrentGeneralStack"] = JsonUtility.ToJson(CurrentGeneralStack);
        pv.Owner.SetCustomProperties(_myCustomProperties);
    }


    /// <summary>
    /// Receive custom properties of the player for stack system. Likely to be called on remote instance
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetCustomPropertiesForStackSystem()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Is Local" + pv.Owner.IsLocal);
        Debug.Log("Other Player City Stack" + pv.Owner.CustomProperties["CurrentCityStack"].ToString());
        JsonUtility.FromJsonOverwrite(pv.Owner.CustomProperties["CurrentCityStack"].ToString(), CurrentCityStack);
        Debug.Log("Other Player General Stack" + pv.Owner.CustomProperties["CurrentGeneralStack"].ToString());
        JsonUtility.FromJsonOverwrite(pv.Owner.CustomProperties["CurrentGeneralStack"].ToString(), CurrentGeneralStack);
    }


    /// <summary>
    /// Get specific stack for the required name
    /// </summary>
    /// <param name="generalName">General stack name for which data is required</param>
    /// <returns></returns>
    public StackClass GetSpecificGeneralStack(string generalName)
    {
        return CurrentGeneralStack.stacks.Find((ob) => ob.name == generalName);

    }

}
