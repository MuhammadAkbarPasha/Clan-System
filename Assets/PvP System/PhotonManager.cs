using System.Collections.Generic;
using ExitGames.Client.Photon;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using BGGamesCore.Matchmaker;
public class PhotonManager : MonoBehaviourPunCallbacks
{


    /// <summary>
    /// Ongoing reconnection time 
    /// </summary>
    [SerializeField] int currentTryingToReconnectTime;
    /// <summary>
    /// Current arena name. In which both players belong
    /// </summary>
    [SerializeField] string CurrentArena;
    /// <summary>
    /// Current level being played
    /// </summary>
    [SerializeField] int LevelNumber = 0;
    /// <summary>
    /// Maximum time for reconnection
    /// </summary>
    [SerializeField] int ReconnectingTime = 30;
    /// <summary>
    /// This class coroutines reference 
    /// </summary>
    IEnumerator coroutine;
    /// <summary>
    /// PhotonManage static instance
    /// </summary>
    public static PhotonManager Instance;
    /// <summary>
    /// Key used to point to arena custom property of the room
    /// </summary>
    public const string ArenaKey = "Arena";
    /// <summary>
    /// Key used to point to level number custom property of the room
    /// </summary>
    public const string LevelNumberKey = "LevelNumber";
    /// <summary>
    /// Enable pvp for use or not
    /// </summary>
    public bool UsePVP = false;
    /// <summary>
    /// Minimum waiting time for other players to join room
    /// </summary>
    public int minimumTimeForWaitingForOthersPlayersToJoinYourRoom;
    ///// Getters and Setters
    public int levelNumber
    {
        get { return LevelNumber; }
    }
    public string currentArena
    {
        get { return CurrentArena; }
    }

    #region Reconnection Section Fields
    [SerializeField] bool rejoinCalled;
    [SerializeField] bool reconnectCalled;
    [SerializeField] bool inRoom;
    [SerializeField] bool wasInRoom;
    DisconnectCause previousDisconnectCause;
    DisconnectCause CurrentCause;

    ///// Getters and Setters
    public bool InRoom
    {
        get { return inRoom; }
        set { inRoom = value; }
    }
    public bool WasInRoom
    {
        get { return wasInRoom; }
        set { wasInRoom = value; }
    }
    #endregion
    [ContextMenu("LeaveRoomImmediate")]
    void LeaveRoomImmediate()
    {
        LeaveRoom();
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    /// <summary>
    ///  This function is called whenever the room is created.
    ///  First this function check if the room is full or not by comparing playerList length.
    ///  Then if the room is not full, it start to find for players for a proper given time.
    ///  If it can't find new player, it add the AI in the room and match is started locally
    ///  Also it make call to leave the room.
    /// 
    /// </summary>
    void CheckIfThereAreEnoughPlayersInRoom() // Being Called In Inovke OnRoomCreated
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {

            ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nNot Player Already In Room";

            return;
        }
        else if (PhotonNetwork.PlayerList.Length == 1)
        {
            MatchmakingSystem.Instance.StartFindRoutine();
            LeaveRoom();
            ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nNot Enough PLayers Switching To Local";

            return;
        }
    }


    /// <summary>
    /// Check if player is stuck in infinite matchmkaing, this could happen because of various reasons.
    /// This function checks if such a scenario has occured. If it has then this function makes player exit the room 
    /// and return to finding to AI to play with 
    /// </summary>
    void LeaveRoomIfStuckOnMatchMaking() // Being Called In Inovke OnRoomCreated
    {
        if (!MatchmakingSystem.Instance.notStuck)
        {
            MatchmakingSystem.Instance.StartFindRoutine();
            LeaveRoom();
            ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nStuckInInfiniteMatchMaking";
            return;
        }
    }


    /// <summary>
    /// Start reconnection time 
    /// </summary>
    /// <returns></returns>
    IEnumerator SetTimerValue()
    {
        MultiplayerUIManager.Instance.TurnObject(false);
        MultiplayerUIManager.Instance.UpdateTimerPanelDisplay(true);
        currentTryingToReconnectTime = ReconnectingTime;
        while (currentTryingToReconnectTime >= 0 && !inRoom)
        {
            MultiplayerUIManager.Instance.UpdateTimerValue(currentTryingToReconnectTime);
            currentTryingToReconnectTime--;
            yield return new WaitForSeconds(1);
        }
        MultiplayerUIManager.Instance.TurnObject(true);
    }


    /// <summary>
    ///  This function set the player's boolean status regarding joined room on local instance.
    ///  Call the function "OnMatchEnd" from another script.
    ///  And than also make call to leave the room on photon server.
    /// 
    /// </summary>
    public void LeaveRoom()
    {
        CancelInvoke(nameof(LeaveRoomIfStuckOnMatchMaking));
        //  CancelInvoke("KeepCheckingDuringMatchMaking");
        MatchmakingSystem.Instance.FoundMatch = false;
        wasInRoom = false;
        inRoom = false;
        PlayersScoreController.Instance.MatchInProgressToggle(false);
        MatchmakingSystem.Instance.OnMatchEnd();
        if (PhotonNetwork.InRoom)
        {
            PlayersScoreController.Instance.ClearCustomPropertiesOfPlayers();
            PhotonNetwork.LeaveRoom();
        }
    }


    /// <summary>
    /// Cancel invoke when match has been found.
    /// </summary>
    public void CancelInvokesOnStartGame()
    {
        CancelInvoke(nameof(LeaveRoomIfStuckOnMatchMaking));
        //   CancelInvoke("KeepCheckingDuringMatchMaking");
    }


    /// <summary>
    /// 
    ///  This function is called only one time when the attached object is set active in the scene
    ///  It check if the player is connected to photon network or not.
    ///  If not, it try to connect to photon network.
    ///  Otherwise it do nothing, just print the statement on console that player is already connected.
    /// 
    /// </summary>
    public void Start()
    {

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("We are connected already.");
        }
    }


    /// <summary>
    ///  Photon call back function when the player is connectd to master server and ready for match making.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "Server Is : " + PhotonNetwork.BestRegionSummaryInPreferences;
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nOnConnectedToMaster";
        if (this.reconnectCalled)
        {
            Debug.Log("Reconnect successful");
            this.reconnectCalled = false;
        }
    }



    /// <summary>
    ///  It update the UI on the basis of given inputs.
    ///  Then show the timer to reconnect.
    ///  If player comes/does not come within given time, it update the UI accordingly.
    /// </summary>

    public void RetryPhotonConnect()
    {
        coroutine = SetTimerValue();
        this.HandleDisconnect(CurrentCause);
        StartCoroutine(coroutine);
    }


    /// <summary>
    /// 
    ///  This function is called when the player is disconnected.
    ///  It update the player's boolean status regarding joined room on local instance.
    ///  Start the coroutine of reconnecting time.
    ///  Set the cause of disconnection.
    /// 
    /// </summary>
    /// <param name="cause"></param>

    public override void OnDisconnected(DisconnectCause cause)
    {

        inRoom = false;
        // show timer in revercse order
        Debug.Log("Internet Disconnected on HomeScreen ?" + !wasInRoom);

        if (this.wasInRoom && coroutine == null)
        {
            //StartCoroutine(SetTimerValue()); 
            coroutine = SetTimerValue();
            StartCoroutine(coroutine);
        }
        if (this.rejoinCalled)
        {
            this.rejoinCalled = false;
        }
        else if (this.reconnectCalled)
        {
            this.reconnectCalled = false;
        }
        CurrentCause = cause;
        this.HandleDisconnect(cause);
        this.previousDisconnectCause = cause;
    }

    public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnJoinedLobby()
    {
    }

    public void OnLeftLobby()
    {
    }
    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }


    /// <summary>
    /// 
    ///  Photon call back function when the server is unable to create a room.
    ///  If so, it call the function to join random room.
    /// 
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        JoinRandomRoom();
    }


    /// <summary>
    /// 
    ///  Photon's call back function.
    ///  It update the player's boolean status regarding joined room on local instance if
    ///    the player is unable to join the room.
    ///
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (this.rejoinCalled)
        {

            ArenaAndMatchMakingBridge.Instance.DebugText.text += string.Format("Quick rejoin failed with error code: {0} & error message: {1}", returnCode, message);
            if (returnCode == 32758)
            {
                ArenaAndMatchMakingBridge.Instance.DebugText.text += string.Format("Game Ended Found on Internet Reconnected,room doesnt exist, either other player won and exited the room or they also disconnected");
                this.inRoom = true;
                wasInRoom = true;
                OnRejoinedCalls();
                coroutine = null;
                LeaveRoom();
            }
            this.rejoinCalled = false;
        }
    }


    /// <summary>
    /// 
    ///  Photon's call back function if player is unable to join random room in a given time, it creates its own room.
    /// 
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>

    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nFailed To Join Room";
        CreateNewRoom();
    }


    /// <summary>
    /// 
    ///  Photon's call back function if the player left the room.
    ///  Set the player boolean status ragarding joined room in a local instance.
    /// </summary>

    public override void OnLeftRoom()
    {

        this.inRoom = false;
    }
    /// <summary>
    /// 
    ///  Photon call back function when the room is created.
    /// 
    /// </summary>
    public override void OnCreatedRoom()
    {
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nCreate room successfully";
    }

    /// <summary>
    /// 
    ///  If player found a match, it does nothing.
    ///  But if player unable to find a match, it make a call to leave the room after given time.
    /// 
    /// </summary>
    public void MatchMakingFailedCheck()
    {


        if (MatchmakingSystem.Instance.FoundMatch)
        {
            return;
        }
        else
        {
            ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nInfinite MatchMaking quickfix";
            MatchmakingSystem.Instance.StartFindRoutine();
            LeaveRoom();
        }
    }


    /// <summary>
    /// 
    ///  It simply set the photon's room properties which includes STANDARD ROOM PROPERTIES and LOBBY PROPERTIES.
    ///  And thean make a call to create new room with these properties.
    /// 
    /// </summary>
    public void CreateNewRoom()
    {
        MatchmakingSystem.Instance.OnMatchEnd(); // Clear Old Game

        CurrentArena = ArenaAndMatchMakingBridge.Instance.GetArenaName(PlayFabManager.Instance.currentMember.MemberPersonalData.Level);
        PlayersScoreController.Instance.SetUpPuzzles(ArenaAndMatchMakingBridge.Instance.rootAllCities.AllCities.Find((ob) => ob.CityName == currentArena));
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nArena For Which Room Created: " + CurrentArena;
        RoomOptions roomOptions = new RoomOptions();
        LevelNumber = 10; // Random Level Number Being Generate Here, not being used anymore
        ExitGames.Client.Photon.Hashtable filterProperties = new ExitGames.Client.Photon.Hashtable() { { ArenaKey, CurrentArena }, { LevelNumberKey, LevelNumber }, { PlayFabManager.Instance.PlayFabUserId, "" } };
        string[] lobbyProperties = { ArenaKey };
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nRandom Level Is: " + filterProperties[LevelNumberKey];
        roomOptions.CustomRoomPropertiesForLobby = lobbyProperties;
        roomOptions.CustomRoomProperties = filterProperties;
        roomOptions.MaxPlayers = 2;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.EmptyRoomTtl = 5000;
        roomOptions.CleanupCacheOnLeave = false;
        roomOptions.PlayerTtl = (ReconnectingTime + 10) * 1000;
        PhotonNetwork.CreateRoom(null, roomOptions, null);

    }


    /// <summary>
    /// 
    ///  It get arena in which the player is currently playing to find the room in that same arena (argument's arena is unnecessary here). 
    ///  Make a call to join random room on photon server.
    /// 
    /// </summary>
    /// <param name="arena"></param>
    public void JoinRandomRoom(string arena)
    {
        MatchmakingSystem.Instance.OnMatchEnd(); // Clear Old Game

        // CancelInvoke("MatchMakingFailedCheck");
        //  Invoke("MatchMakingFailedCheck", matchingMakingTimeOutTime + minimumTimeForWaitingForOthersPlayersToJoinYourRoom);
        // //
        CurrentArena = ArenaAndMatchMakingBridge.Instance.GetArenaName(PlayFabManager.Instance.currentMember.MemberPersonalData.Level);
        PlayersScoreController.Instance.SetUpPuzzles(ArenaAndMatchMakingBridge.Instance.rootAllCities.AllCities.Find((ob) => ob.CityName == currentArena));

        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nSearching For Room In Arena: " + CurrentArena;
        ExitGames.Client.Photon.Hashtable roomTournament = new ExitGames.Client.Photon.Hashtable { { ArenaKey, CurrentArena } };
        PhotonNetwork.JoinRandomRoom(roomTournament, 0);
    }


    /// <summary>
    /// 
    ///  Photon call back when the new player enter in a room.
    ///  Set the room lobby properties to invisible if the player is master clint (The one who created room).
    ///  And also set that, the room cannot be joined because the player already in the room.
    /// 
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nNew Player Entered Room: " + CurrentArena;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //            Optionally the room also can be set invisible in lobby list:
            //          PhotonNetwork.room.visible = false;
        }
    }


    /// <summary>
    /// 
    ///  Called when the player rejoined the room and update UI accordingly.
    /// 
    /// </summary>
    public void OnRejoinedCalls()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        MultiplayerUIManager.Instance.UpdateTimerValue(0);
        MultiplayerUIManager.Instance.UpdateTimerPanelDisplay(false);
        Debug.Log("Rejoin successful");
        this.rejoinCalled = false;

    }


    /// <summary>
    /// 
    ///  Photon call back function when player joind the room.
    ///  Instanciate player remote instance.
    ///  Make calls to check if the room is full or not.
    /// 
    /// </summary>
    public override void OnJoinedRoom()
    {
        this.inRoom = true;
        wasInRoom = true;
        coroutine = null;
        if (this.rejoinCalled)
        {
            OnRejoinedCalls();
            return;
        }
        CancelInvoke(nameof(CheckIfThereAreEnoughPlayersInRoom));
        GameObject pvpInstance = PhotonNetwork.Instantiate("PlayerPVPInstance", this.transform.position, Quaternion.identity);
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nThis Player Joined Room";
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke(nameof(CheckIfThereAreEnoughPlayersInRoom), minimumTimeForWaitingForOthersPlayersToJoinYourRoom);

        }
        if (!PhotonNetwork.IsMasterClient)
        {

            GetLevelFromProperties();
        }
    }
    public void GetLevelFromProperties()
    {
        LevelNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties[LevelNumberKey];
    }


    /// <summary>
    /// Return the level number of current arena
    /// </summary>
    /// <returns></returns>
    public int GetRandomLevel()
    {

        return ArenaAndMatchMakingBridge.Instance.GetRandomLevel(CurrentArena);

    }
    public void JoinRandomRoom()
    {
        JoinRandomRoom(GameDataManager.Instance.playerData.playerCurrentLeaderboard);
        CancelInvoke(nameof(LeaveRoomIfStuckOnMatchMaking));
        Invoke(nameof(LeaveRoomIfStuckOnMatchMaking), minimumTimeForWaitingForOthersPlayersToJoinYourRoom + 5);
    }
    public void KeepCheckingDuringMatchMaking()
    {
        return;


        // if (!MatchmakingSystem.Instance.FoundMatch)
        // {
        //     // Internet Connection Test

        //     // if (!ConnectionTester.GetInstance().internetPossiblyAvailable)
        //     // {

        //     //     //cancel matchmaking 

        //     //     gamePlayManager.StarScreenClose();
        //     // }
        // }
    }


    /// <summary>
    /// 
    ///  Set the level number from current room properties made on a photon server
    /// 
    /// </summary>
    /// <param name="levelNumber"></param>
    public void SetMapFromProperties(int levelNumber)
    {
        LevelNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties["LevelNumber"];
    }
    public IEnumerator WaitForOtherPlayerToCome()
    {
        yield return new WaitForEndOfFrame();
    }


    #region  Reconnection Section
    /// <summary>
    /// 
    ///  A coroutine to constantly check if player is still in a room or not.
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ConnectionLoopForDevice()
    {
        yield return new WaitForSeconds(2f);
        while (wasInRoom && !inRoom)
        {
            ArenaAndMatchMakingBridge.Instance.DebugText.text += "\ncalling Device Loop";
            HandleDisconnect();
            yield return new WaitForSeconds(2f);
        }
    }


    /// <summary>
    /// 
    ///  Over loaded function of named "HandleDisconnect" with same functionality.
    /// 
    /// </summary>
    /// <param name="cause"></param>
    private void HandleDisconnect(DisconnectCause cause)
    {
        //   ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nInRoom " + inRoom + " WasInRoom" + wasInRoom;

        switch (cause)
        {
            // cases that we can recover from
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.Exception:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.AuthenticationTicketExpired:
            case DisconnectCause.DisconnectByServerReasonUnknown:
            case DisconnectCause.ExceptionOnConnect:
                if (this.inRoom == true || wasInRoom == true)
                {
                    if (currentTryingToReconnectTime >= 0)
                    {
                        //  ArenaAndMatchMakingBridge.Instance.DebugText.text += "\ncalling PhotonNetwork.ReconnectAndRejoin()";
                        this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();

                    }
                    else
                    {
                        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nRejoin Time Run Out";

                    }
                }
                else
                {


                    PhotonNetwork.Reconnect();

                }
                break;
            case DisconnectCause.None:
            case DisconnectCause.OperationNotAllowedInCurrentState:
            case DisconnectCause.CustomAuthenticationFailed:
            case DisconnectCause.DisconnectByClientLogic:
            case DisconnectCause.InvalidAuthentication:
            case DisconnectCause.MaxCcuReached:
            case DisconnectCause.InvalidRegion:
                ArenaAndMatchMakingBridge.Instance.DebugText.text += string.Format("\nDisconnection we cannot automatically recover from, cause: {0}, report it if you think auto recovery is still possible", cause);
                break;
        }
    }



    /// <summary>
    /// 
    ///  Depending upon the cause of disconnection, if reconnection is possible, it tries to reconnect the player
    ///    to photon server otherwise simply break.
    ///    
    /// </summary>
    private void HandleDisconnect()
    {
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nInRoom " + inRoom + " WasInRoom" + wasInRoom;

        switch (CurrentCause)
        {
            // cases that we can recover from
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.Exception:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.AuthenticationTicketExpired:
            case DisconnectCause.DisconnectByServerReasonUnknown:
            case DisconnectCause.ExceptionOnConnect:
                if (this.inRoom == true || wasInRoom == true)
                {
                    ArenaAndMatchMakingBridge.Instance.DebugText.text += "\ncalling PhotonNetwork.ReconnectAndRejoin()";
                    this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                }
                break;
            case DisconnectCause.None:
            case DisconnectCause.OperationNotAllowedInCurrentState:
            case DisconnectCause.CustomAuthenticationFailed:
            case DisconnectCause.DisconnectByClientLogic:
            case DisconnectCause.InvalidAuthentication:
            case DisconnectCause.MaxCcuReached:
            case DisconnectCause.InvalidRegion:
                ArenaAndMatchMakingBridge.Instance.DebugText.text += string.Format("\nDisconnection we cannot automatically recover from, cause: {0}, report it if you think auto recovery is still possible", CurrentCause);
                break;
        }
    }

    #endregion
}

