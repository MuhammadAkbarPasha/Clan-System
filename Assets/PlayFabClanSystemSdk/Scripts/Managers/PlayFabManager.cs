using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using ExitGames.Client.Photon;
using System;
using PlayFab.CloudScriptModels;
using System.Globalization;

public class PlayFabManager : MonoBehaviour
{
    [Header("Playfab Settings")]
    /// <summary>
    /// This user Playfab Master id
    /// </summary>
    public string PlayFabUserId;
    /// <summary>
    /// Title id of the Playfab Title
    /// </summary>
    public string PlayFabTitleId;
    /// <summary>
    /// Static Instance object of PlayFabManager
    /// </summary>
    public static PlayFabManager Instance;
    [SerializeField]
    /// <summary>
    /// Is Facebook Login enable or not
    /// </summary>
    private bool NoFacebookLogging = false;
    [Header("Playfab Player Keys")]
    /// <summary>
    /// Key to refernce Player Data
    /// </summary>
    public string PlayerDataKey = "PlayerData";
    /// <summary>
    /// Key to refernce Game Data
    /// </summary>
    public string GameDataKey = "GameData";
    [Header("Playfab Player Data")]
    /// <summary>
    /// Username of this Player
    /// </summary>
    public string username;
    /// <summary>
    /// Avatar url of this Player
    /// </summary>
    public string avatarUrl;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of Life
    /// </summary>
    private int Lives;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of Energy
    /// </summary>
    private int Energies;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of Dart
    /// </summary>
    private int Darts;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of Coin
    /// </summary>
    private int Coins;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of Hint
    /// </summary>
    private int Hints;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of Timer
    /// </summary>
    private int Timers;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of SpinWheel
    /// </summary>
    private int SpinWheels;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of BonusLife1
    /// </summary>
    private int BonusLife1;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of BonusLife2
    /// </summary>
    private int BonusLife2;
    [SerializeField]
    /// <summary>
    /// Player VirtualCurrency of BonusLife3
    /// </summary>
    private int BonusLife3;
    /// <summary>
    /// Refernce to BonusLifeTime of this PLayer 
    /// </summary>
    public LifeTimer bonusLifeTimer;
    /// <summary>
    /// Refernce to BonusLifeTime of this PLayer 
    /// </summary>
    public LifeTimer vipTimer;
    /// <summary>
    /// Bonus Life Time in string format 
    /// </summary>
    public string timeToDisplay;

    [Header("Country Data")]
    /// <summary>
    /// CountryClass reference of this Player
    /// </summary>
    /// <returns></returns>
    public CountryClass playerCountry = new CountryClass();
    /// <summary>
    /// List of Possible Countries Player can be From
    /// </summary>
    /// <typeparam name="CountryClass"></typeparam>
    /// <returns></returns>
    [SerializeField] List<CountryClass> AvailableCountries = new List<CountryClass>();
    /// <summary>
    /// This is used to find difference between BonusLifeTimer start and end time
    /// </summary>
    TimeSpan timeDifference;
    /// <summary>
    /// Used as one second refernce. For calculations need
    /// </summary>
    /// <returns></returns>
    TimeSpan secondDiff = new TimeSpan(0, 0, 1);
    /// <summary>
    /// Not used much, Only for login with email, for test.
    /// </summary>
    private string password;
    [Header("Playfab Player Group Specific Data")]
    /// <summary>
    /// Refernce to this Player Key object.
    /// </summary>
    /// <returns></returns>
    public Key currentMember = new Key();
    /// <summary>
    /// Did this player logged in for the first time.
    /// </summary>
    private bool first_login = false;
    /// <summary>
    /// Has the player logged in with facebook
    /// </summary>
    bool isFacebookLoggedIn=false;
    [Header("Photon Chat App Id")]
    /// <summary>
    /// Photon Chat App Id
    /// </summary>
    public string PhotonAppId;
    /// <summary>
    /// This the token which is received when player is authenticated to use PhotonChat
    /// </summary>
    private string chatToken;
    /// <summary>
    /// String object holding the data about type of last login
    /// </summary>
    public string sessionManagementString;



    /// <summary>
    /// Sprite object of Player Avatar
    /// </summary>
    public Sprite PlayerAvatar;
    #region Getter_Setter 
    public string getAvatar()
    {
        return this.avatarUrl;
    }
    public void setAvatar(string url)
    {
        this.avatarUrl = url;
    }

    public int getLives()
    {
        return this.Lives;
    }
    public void setLives(int lives)
    {
        this.Lives = lives;
    }

    public int getEnergies()
    {
        return this.Energies;
    }
    public void setEnergies(int energies)
    {
        this.Energies = energies;
    }

    public int getDarts()
    {
        return this.Darts;
    }
    public void setDarts(int darts)
    {
        this.Darts = darts;
    }
    public int getCoins()
    {
        return this.Coins;
    }
    public void setCoins(int coins)
    {
        this.Coins = coins;
    }
    public int getSpinWheels()
    {
        return this.SpinWheels;
    }
    public void setSpinWheels(int sw)
    {
        this.SpinWheels = sw;
    }
    public int getTimer()
    {
        return this.Timers;
    }
    public void setTimer(int tms)
    {
        this.Timers = tms;
    }
    public int getHint()
    {
        return this.Hints;
    }
    public void setHint(int hts)
    {
        this.Hints = hts;
    }
    public int getBonusLife1()
    {
        return BonusLife1;
    }
    public int getBonusLife2()
    {
        return BonusLife2;
    }
    public int getBonusLife3()
    {
        return BonusLife3;
    }
    public void setUsername(string uname)
    { // use this to set username for android login case
        Debug.Log("before" + this.username);
        this.username = uname;
        Debug.Log("after" + this.username);
    }

    public void setCurrency(ClanSystem.VirtualCurrency.CloudResponse.Response response)
    {
        this.Lives = int.Parse(response.LF);
        this.Timers = int.Parse(response.TM);
        this.Hints = int.Parse(response.HT);
        this.SpinWheels = int.Parse(response.SW);
        this.Coins = int.Parse(response.CO);
        this.BonusLife1 = int.Parse(response.B1);
        this.BonusLife2 = int.Parse(response.B2);
        this.BonusLife3 = int.Parse(response.B3);
        this.Energies = int.Parse(response.EN);
        this.Darts = int.Parse(response.DT);
    }
    #endregion
    #region Bonus_Life_Timer_Functionality
    /// <summary>
    /// This is called whenever any of the BonusLife is consumed. I.e. ConsumeB1,2,3 , CheckIfExpired or  ExpireBonusLife call in StoreSystemController.
    /// </summary>
    public void Bonus_Life_timer_init()
    {
        if (!bonusLifeTimer.useBonusLife)
        {
            timeToDisplay = "00:00";
            return;
        }
        DateTime startTime = Convert.ToDateTime(bonusLifeTimer.startTime);
        DateTime endTime = Convert.ToDateTime(bonusLifeTimer.endTime);
        timeDifference = (endTime - startTime);
        InvokeRepeating(nameof(BonusLifeTimer), 1, 1);

    }


    /// <summary>
    /// BonusLifeTimer Expirer
    /// </summary>
    public void BonusLifeTimer()
    {
        timeDifference = timeDifference.Subtract(secondDiff);
        timeToDisplay = timeDifference.ToString();

        if (timeDifference.TotalSeconds < 1)
        {
            StoreSystemController.Instance.ExpireBonusLife();
        }

    }
    #endregion

    #region  PlayerAvatarFunctionality
    /// <summary>
    /// Download Avatar In CoRoutine and setup
    /// </summary>
    /// <returns></returns>
    public IEnumerator DownloadAvatar()
    {
        Debug.LogError("PlayfabManager DownloadAvatar0000.......:" + getAvatar());
        if (Uri.IsWellFormedUriString(getAvatar(), UriKind.Absolute))
        {
            WWW www = new WWW(avatarUrl);
            yield return www;
            PlayerAvatar = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
            // Debug.LogError("DownloadAvtar1");
            //PlayPabUiController.Instance.DisplayMyFbProfilePic(PlayerAvatar);
            UIBattleLobby.Instance.SetPlayerSprite(PlayerAvatar);
        }
        else
        {
            Debug.LogError("PlayfabManager DownloadAvatar11111.......:" + getAvatar());
            PlayerAvatar = Sprite.Create(null, new Rect(0, 0, 200, 200), new Vector2(0.5f, 0.5f), 32);  // you win need to add your own functionality here for default avatars
                                                                                                        // Debug.LogError("DownloadAvtar2");
                                                                                                        //PlayPabUiController.Instance.DisplayMyFbProfilePic(PlayerAvatar);
            UIBattleLobby.Instance.SetPlayerSprite(PlayerAvatar);
        }
    }

    [ContextMenu("Update Avatar")]
    /// <summary>
    /// Set Avatar URL on Playfab and also call for download
    /// </summary>
    /// <param name="url">Avatar URL on Playfab</param>
    public void SetPlayerAvatar(string url)
    {
        Debug.LogError("New Avatar Is" + url);

        UpdateAvatarUrlRequest updateAvatarUrlRequest = new UpdateAvatarUrlRequest
        {
            ImageUrl = url

        };
        PlayFabClientAPI.UpdateAvatarUrl(updateAvatarUrlRequest,
            resultCallback =>
            {
                setAvatar(url);
                StartCoroutine(DownloadAvatar());
            },
            errorCallback =>
            {

                Debug.Log(errorCallback.Error);
            }
            );
    }
    #endregion
    #region Login Management 
    void Awake()
    {
        Instance = this;
        GF_SaveLoad.LoadProgress();
    }

    public void signOut()
    {
        GF_SaveLoad.DeleteProgress();
        PlayerPrefs.DeleteAll();
        currentMember = new Key();
        ClanSystemController.Instance.ClearData();
        TradeSystemController.Instance.clearData();
        PlayFabUserId = "";
        username = "";
        password = "";
        //PlayPabUiController.Instance.OnSignOut();
        setEnergies(0);
        setLives(0);
        setCoins(0);
        setSpinWheels(0);
        setTimer(0);
        setHint(0);
        InboxSystemController.Instance.SignOut();
        //IsFacebookSignOut = "true";
        //chirag.......
        FacebookManager.Instance.LogoutFromFacebook();
    }
    public string IsFacebookSignOut
    {
        get
        {
            return PlayerPrefs.GetString("IsFacebookSignOut", "");
        }
        set
        {
            PlayerPrefs.SetString("IsFacebookSignOut", value);
        }
    }
    /// <summary>
    /// Choose the flow of login based on previous Login
    /// </summary>
    public void SessionManagement()
    {
        PlayFabSettings.TitleId = PlayFabTitleId;
        sessionManagementString = PlayerPrefs.GetString("Login_Type");
        if (!PlayerPrefs.HasKey("Login_Type"))
        {
            LoginGuest();
            return;
        }

        if (!GF_SaveLoad.CheckIfFileExists() && !PlayerPrefs.HasKey("Login_Type"))
        {
            Debug.Log("No Saved Data Exists");
            return;
        }
        switch (PlayerPrefs.GetString("Login_Type"))
        {
            case "Facebook":
                {
                    if (NoFacebookLogging)
                        LoginGuest();
                    else
                    {

                        if (!PlayerPrefs.HasKey("FacebookToken"))
                        {
                            FaceBook_Login();
                        }
                        else
                        {
                            LoginWithFacebook();
                        }
                    }
                }
                break;
            case "Guest":
                {
                    LoginGuest();
                }
                break;
            case "LoginWithPlayfab":
                {
                    Debug.Log("How Many Times Here");
                    this.username = SaveData.Instance.Username;
                    LoginWithPlayFab(this.username, this.password);

                }
                break;

        }
    }
    /// <summary>
    /// Facebook Login Caller
    /// </summary>
    public void FaceBook_Login()
    {
        Debug.LogError("FaceBook_Login1");
        FacebookManager.Instance.FBLogin();
    }
    /// <summary>
    /// Login with playfab api caller. Test purposes only.
    /// </summary>
    /// <param name="username">Player's required username</param>
    /// <param name="password">Player's required password</param>
    public void LoginWithPlayFab(string username, string password)
    {
        Debug.Log(1);
        PlayerPrefs.SetString("Login_Type", "LoginWithPlayfab");
        if (!(SaveData.Instance.Username.Length < 1) && !(SaveData.Instance.Password.Length < 1) && GF_SaveLoad.CheckIfFileExists())
        {
            username = SaveData.Instance.Username;
            password = SaveData.Instance.Password;
            Debug.Log(2 + " " + SaveData.Instance.Password + "" + SaveData.Instance.Username);
        }
        else
        {
            SaveData.Instance = new SaveData(username, password);
            GF_SaveLoad.SaveProgress();
            Debug.Log(3 + " " + SaveData.Instance.Password + "" + SaveData.Instance.Username);
        }
        this.username = username;
        this.password = password;
        PlayFabSettings.TitleId = PlayFabTitleId;

        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest
        {
            Password = password,
            TitleId = PlayFabTitleId,
            Username = username,
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFail);
    }

    /// <summary>
    /// Login with Device. Used for both android and ios
    /// </summary>
    public void LoginGuest()
    {
        Debug.Log("Title id is : " + PlayFabSettings.TitleId);
        PlayerPrefs.GetString("Login_Type", "Guest");

#if UNITY_ANDROID || ANDROID
        LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDevice = SystemInfo.deviceModel,
            AndroidDeviceId = ReturnDeviceId(),
            TitleId = PlayFabSettings.TitleId,
            OS = SystemInfo.operatingSystem,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, DeviceLoginSuccess,
                (CustomIdLoginError) =>
                {
                    Debug.Log("Error logging in player with custom ID: ");
                    Debug.Log(CustomIdLoginError.ErrorMessage);
                }
              );
#endif

#if UNITY_IOS || IOS

        //IOS here

        LoginWithIOSDeviceIDRequest iosrequest = new LoginWithIOSDeviceIDRequest { TitleId = PlayFabSettings.TitleId, DeviceId = ReturnDeviceId(), CreateAccount = true, OS = SystemInfo.operatingSystem };
        PlayFabClientAPI.LoginWithIOSDeviceID(  iosrequest,DeviceLoginSuccess,
              (CustomIdLoginError) =>
              {
                  Debug.Log("Error logging in player with custom ID: ");
                  Debug.Log(CustomIdLoginError.ErrorMessage);
              }
   );
#endif
    }

    /// <summary>
    /// Get deviceID. Works for both ios and android.
    /// </summary>
    /// <returns>Returns ID for android or ios</returns>
    public string ReturnDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
    /// <summary>
    /// Callback for successful login on Device
    /// </summary>
    /// <param name="deviceLoginResult"> LoginResult Object</param>
    public void DeviceLoginSuccess(LoginResult deviceLoginResult)
    {
        PlayFabUserId = deviceLoginResult.PlayFabId;
        if (deviceLoginResult.NewlyCreated)
        {
            PlayerPrefs.SetString("Login_Type", "Guest");
            sendDataOnAccountRegisteration();
            //UiController.Instance.showSetUserNamePanelForAndroidUser();
            //set profile on playfab.......
            // SetPlayerAvatar(EditProfileScript.Instance.ProfileIndex.ToString());
            //pehele ye call kiya hua tha.......
            //SetPlayerAvatar((1).ToString());
            //return;
        }
        else
        {
            Debug.LogError("device login success getDataOnAccountLogin()");
            PlayerPrefs.SetString("Login_Type", "Guest");
            GetDataOnAccountLogin();
        }
        ClanSystemController.Instance.GetCurrentPlayerDataForGroup(PlayFabUserId);
        Debug.Log("i show result here " + deviceLoginResult);
        // //UiController.Instance.OpenGroupOptions();
        // if (!PlayPabUiController.Instance.IsEditorOrNot)
        // {
        //     PlayPabUiController.Instance.loginPanel.SetActive(false);
        // }
        //TournamentSystem.Instace.GetTeamTournamentForPlayer();
        //if (PlayPabUiController.Instance.IsStarTournamnetStarted != 0)
        //{
        //    TournamentSystem.Instace.GetPersonalTournamentEndTime();
        //}
        // PlayPabUiController.Instance.StarTournamentGetEndTime();

        RequestCurrency();
        GetSpinWheelItems();
        StoreSystemController.Instance.getIAPItems();
        StoreSystemController.Instance.CheckIfBonusLifeExpired();
        StoreSystemController.Instance.getVipTime();
        StoreSystemController.Instance.RegisterUserLoginTimeStamp();
        EventControllerScript.Instance.GetTodayEventDetails();
        EventControllerScript.Instance.GetDailyEventEndTime();
        TournamentSystem.Instace.getPersonalTournament();
        TournamentSystem.Instace.GetTeamEvent();
        EnergySystem.Instance.GetLevels();
        EnergySystem.Instance.ResumeTimer();
        InboxSystemController.Instance.OnSignin();
        GetPlayerCountry();
        StartCoroutine(FakePlayerHandler.Instance.GetLeaderboard());
        //ClanSystemController.Instance.ListMembershipRequestForCurrentMember();

        //this method are used to get all leaderboard.......
        LeaderboardSystem.Instance.GetLocalLeaderboard();
        LeaderboardSystem.Instance.GetLeaderboardTop100();
        LeaderboardSystem.Instance.GetLeaderboardOfFriends();

        ArenaAndMatchMakingBridge.Instance.GetAllCitiesData();
        ArenaAndMatchMakingBridge.Instance.GetAllGeneralsData();
        ArenaAndMatchMakingBridge.Instance.GetLevelsToBeFiltered();
        StartCoroutine(PlayerLevelsDataManager.Instance.GetPlayerAllStacks());

        //this method are used for piggy bank.......
        BankController.Instance.LoginCall();

        //this method is used for cities and collection.......
        //CitiesAndCollectiblesManager.Instance.LoginCall();

        //this line are used to Daily Reward.......umesh
        //this method are used to get current utc time.......
        //Debug.LogError("GetTime 111111111111111111111111.......");
        //this method are used to get latest version and check force update or not.......
        
    }
    /// <summary>
    /// Requests the Playfab server for the current player country and sets up CountryClass object playerCountry
    /// </summary>
    public void GetPlayerCountry()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetPlayerCountry",
        }, resultCallback =>
        {
            try
            {
                Debug.Log("GetPlayerCountry:" + resultCallback.FunctionResult.ToString());
                playerCountry = AvailableCountries.Find((ob) =>
                ob.CountryCode == resultCallback.FunctionResult.ToString());
            }
            catch
            {
                Debug.Log("Unexpected Output");
            }
        }
        , errorCallback =>
        {
            Debug.LogError("errorCallback:" + errorCallback.ErrorMessage);
        }
        );
    }
    /// <summary>
    /// Set playab UserName on Playfab Server
    /// </summary>
    /// <param name="userName">Username to set for the current Player</param>
    public void SetUserNameOnPlayFab(string userName)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,

            resultCallback =>
            {
                Debug.Log("Successfully Changed");
            }, errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            }
            );
    }

    /// <summary>
    /// SetUserNameOnPlayFab overloaded function.
    /// Used when username is to be set directly from us or after altering the username field 
    /// /// </summary>
    public void SetUserNameOnPlayFab()
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            resultCallback =>
            {
                if (PlayerPrefs.GetString("Login_Type") == "Guest")
                {
                    ClanSystemController.Instance.GetCurrentPlayerDataForGroup(PlayFabUserId);
                    Debug.Log("i show result here " + resultCallback);
                    //UiController.Instance.OpenGroupOptions();

                    RequestCurrency();
                    GetSpinWheelItems();
                    //TournamentSystem.Instace.GetTeamTournamentForPlayer();
                    //if (PlayPabUiController.Instance.IsStarTournamnetStarted != 0)
                    //{
                    //    TournamentSystem.Instace.GetPersonalTournamentEndTime();
                    //}
                    StoreSystemController.Instance.getIAPItems();
                    StoreSystemController.Instance.CheckIfBonusLifeExpired();
                    StoreSystemController.Instance.getVipTime();
                    StoreSystemController.Instance.RegisterUserLoginTimeStamp();
                    EventControllerScript.Instance.GetTodayEventDetails();
                    EventControllerScript.Instance.GetDailyEventEndTime();
                    UiController.Instance.TurnOffSetUserNamePanelForAndroidUser();
                    TournamentSystem.Instace.getPersonalTournament();
                    TournamentSystem.Instace.GetTeamEvent();
                    EnergySystem.Instance.GetLevels();
                    EnergySystem.Instance.ResumeTimer();
                    InboxSystemController.Instance.OnSignin();
                    GetPlayerCountry();
                    StartCoroutine(FakePlayerHandler.Instance.GetLeaderboard());
                    //ClanSystemController.Instance.ListMembershipRequestForCurrentMember();

                    //this method are used to get all leaderboard.......
                    LeaderboardSystem.Instance.GetLocalLeaderboard();
                    LeaderboardSystem.Instance.GetLeaderboardTop100();
                    LeaderboardSystem.Instance.GetLeaderboardOfFriends();

                    ArenaAndMatchMakingBridge.Instance.GetAllCitiesData();
                    ArenaAndMatchMakingBridge.Instance.GetAllGeneralsData();
                    ArenaAndMatchMakingBridge.Instance.GetLevelsToBeFiltered();
                    StartCoroutine(PlayerLevelsDataManager.Instance.GetPlayerAllStacks());

                    //this method are used for piggy bank.......
                    BankController.Instance.LoginCall();

                    //this method is used for cities and collection.......
                    //CitiesAndCollectiblesManager.Instance.LoginCall();

                    //this line are used to Daily Reward.......umesh


                    //this method are used to get current utc time.......
                    // Debug.LogError("GetTime 222222222222222222222222.......");
                    //this method are used to get latest version and check force update or not.......
                }
            }, errorCallback =>
            {
                Debug.Log(errorCallback.ErrorMessage);
            }
            );
    }
    /// <summary>
    /// Login Callback for on successful login with Facebook and Login With Playfab
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result)
    {
        // AndroidIAPExample.Instance.RefreshIAPItems();

        PlayFabUserId = result.PlayFabId;
        if (first_login)
        {
            sendDataOnAccountRegisteration();
            first_login = false;
            //SetUserNameOnPlayFab(); // have commented this out for setting display name only on joining group functionality
            if (PlayerPrefs.GetString("Login_Type") == "Facebook")
            {
                SetUserNameOnPlayFab();
            }
            else
            {
                Debug.LogError("Playfabmanager OnLoginSuccess setplayer avatar.......:");
                //set profile on playfab.......
                //pehele ye call kiya hua tha.......
                //SetPlayerAvatar((2).ToString());
            }
        }
        else
        {
            GetDataOnAccountLogin();
        }
        //GetAccountInfo();
        Debug.Log("i show result here " + result);
        OnLoginCalls();
    }
    /// <summary>
    /// Change state of chat. Other devs requirement.
    /// </summary>
    /// <param name="state">State to set</param>
    public void ChangeChatState(bool state)
    {
        currentMember.MemberPersonalData.ChatEnabled = state;
        UpdateUserReadOnlyData(JsonUtility.ToJson(currentMember.MemberPersonalData));
        if (state) ChatManager.Instance.OnChatEnabled();
        else ChatManager.Instance.OnChatDisabled();
    }
    /// <summary>
    /// Update UserReadOnlyData on Server after changes in MemberData field of currentMember object
    /// </summary>  
    public void SendPlayerData()
    {
        UpdateUserReadOnlyData(JsonUtility.ToJson(currentMember.MemberPersonalData));
    }
    /// <summary>
    /// Udate User Data on Playfab
    /// </summary>
    public void SendGameData()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        var sendUserData = new UpdateUserDataRequest { Data = dict, Permission = UserDataPermission.Public };
        PlayFabClientAPI.UpdateUserData(sendUserData, successfulUpdate, failUpdate);
    }
    /// <summary>
    /// successfulUpdate Callback
    /// </summary>
    /// <param name="result">UpdateUserDataResult object</param>
    private void successfulUpdate(UpdateUserDataResult result)
    {
        Debug.LogError("Updated data success" + result);
    }

    /// <summary>
    /// failedUpdate Callback
    /// </summary>
    /// <param name="result">UpdateUserDataResult object</param>

    private void failUpdate(PlayFabError error)
    {
        Debug.Log("Updated data fail" + error);
    }
    /// <summary>
    /// Login failure Callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    private void OnLoginFail(PlayFabError error)
    {
        Debug.Log("i show result here " + error.Error);
        //PlayerPrefs.DeleteAll();
        switch (error.Error.ToString())
        {
            case "AccountNotFound":
                {
                    RegisterAccount();
                }
                break;
            default:
                {
                    signOut();
                }
                break;
        }
    }
    /// <summary>
    /// RegisterPlayFabUser api caller
    /// </summary>
    public void RegisterAccount()
    {
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest
        {
            Username = this.username,
            RequireBothUsernameAndEmail = false,
            Password = this.password,
            TitleId = PlayFabTitleId
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, (RegisterPlayFabUserResult) =>
        {
            first_login = true;
            LoginWithPlayFab(this.username, this.password);
        }, OnLoginFail);
    }

    //
    ////
    /// <summary>
    ///this run on account registeration for both device and username/password login . no username will be send in device's case and "guest" username will be user
    ///also this is only for player title data not actual username
    /// </summary>
    /// <param name="DummyUserName">Default UserName</param>
    public void sendDataOnAccountRegisteration()
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest()
        {
            PlayFabId = PlayFabUserId
        };
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            currentMember = new Key(result.AccountInfo.TitleInfo.TitlePlayerAccount.Id,
            result.AccountInfo.TitleInfo.TitlePlayerAccount.Id, 1, 0, 64, 0);
            UpdateUserReadOnlyData(JsonUtility.ToJson(currentMember.MemberPersonalData));
        }, failUpdate);
    }
    /// <summary>
    /// UpdateUserReadOnlyData CloudScript function caller
    /// </summary>
    /// <param name="playerData">MemberData in json</param>
    public void UpdateUserReadOnlyData(string playerData)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "UpdateUserReadOnlyData",
            FunctionParameter = new
            {
                PlayerData = playerData
            }
        }, CloudScriptSuccess
        , CloudScriptFailure
        );
    }
    /// <summary>
    /// Get current user UserReadOnlyData
    /// </summary>
    public void GetUserReadOnlyData()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetUserReadOnlyData"

        }, resultCallback =>
        {

            Debug.Log(resultCallback.FunctionName);
            Debug.Log(resultCallback.FunctionResult);

            GetAccountInfo();
            if (resultCallback.FunctionResult == null)
                return;
            currentMember.MemberPersonalData = JsonUtility.FromJson<MemberData>(resultCallback.FunctionResult.ToString());
            if (!resultCallback.FunctionResult.ToString().Contains("ChatEnabled"))
            {
                currentMember.setChatEnabled(true);
            }
            EnergySystem.Instance.setplayerLevel(currentMember.MemberPersonalData.Level);
        }
        , CloudScriptFailure
        );
    }
    /// <summary>
    /// CloudScript successful call callback  
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult Object</param>
    public void CloudScriptSuccess(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
    {
        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);
        switch (result.FunctionName)
        {
            case "GetUserReadOnlyData":
                {
                    //Processing on response here
                }
                break;
            case "UpdateUserReadOnlyData":
                {
                    //Processing on response here
                }
                break;
        }
    }
    /// <summary>
    /// CloudScript failed call callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void CloudScriptFailure(PlayFabError error)
    {
        Debug.Log(error.ErrorDetails);
    }
    /// <summary>
    /// Call for data for current player
    /// </summary>
    public void GetDataOnAccountLogin()
    {
        GetUserReadOnlyData();
        GetGameDataApiCall();
    }
    /// <summary>
    /// Call for game data for current player
    /// </summary>
    void GetGameDataApiCall()
    {
        //if (GameManager.IsGetDataFromServer == 0)
        {
            List<string> listofKeys = new List<string>();
            listofKeys.Add(PlayerDataKey);
            var userDataRequest = new GetUserDataRequest { Keys = listofKeys, PlayFabId = PlayFabUserId };
            PlayFabClientAPI.GetUserData(userDataRequest, OnDataReceived, OnDataNotReceived);
        }
    }
    /// <summary>
    /// Avatar Image object coversion
    /// </summary>
    /// <param name="tex">image in Texture2D format</param>
    /// <returns>image in Base64String format</returns>
    public string EncodeImageToBase64(Texture2D tex)
    {
        byte[] bytes;
        bytes = tex.EncodeToPNG();
        string enc = Convert.ToBase64String(bytes);
        return enc;
    }
    /// <summary>
    /// Setup player profile picture caller for FacebookManager class.
    /// </summary>
    /// <param name="tex">image in string format</param>
    /// <returns></returns>
    public IEnumerator UploadPlayerProfilePictureAsAvatar(string tex)
    {
        Debug.Log("Coming here to updated Avatar 1");
        while (!PlayFabClientAPI.IsClientLoggedIn() && !isFacebookLoggedIn)
        {
            yield return new WaitForSeconds(.25f);
        }
        Debug.Log("Coming here to updated Avatar 2");
        Debug.LogError("Playfabmanager UploadPlayerProfilePictureAsAvatar setplayer avatar.......:" + tex);
        SetPlayerAvatar(tex);
        //PlayPabUiController.Instance.fbProfilePicAvtarUrl = tex;
        //SetPlayerAvatar(EncodeImageToBase64(tex));
    }
    /// <summary>
    /// GetAccountInfo api caller
    /// </summary>
    public void GetAccountInfo() // fixed current member id issue 
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest()
        {
            PlayFabId = PlayFabUserId
        };
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            currentMember.setTitleId(result.AccountInfo.TitleInfo.TitlePlayerAccount.Id);

            if (result.AccountInfo.TitleInfo.DisplayName != null)
            {
                setUsername(result.AccountInfo.TitleInfo.DisplayName);
                currentMember.setGroupUsername(result.AccountInfo.TitleInfo.DisplayName);
            }
            Debug.LogError("result.AccountInfo.TitleInfo.AvatarUrl:" + result.AccountInfo.TitleInfo.AvatarUrl);
            if (result.AccountInfo.TitleInfo.AvatarUrl != null)
            {
                setAvatar(result.AccountInfo.TitleInfo.AvatarUrl);
                StartCoroutine(DownloadAvatar());
                // UiController.Instance.DisplayMyFbProfilePic();
            }
            else
            {
                Debug.LogError("Playfabmanager GetAccountInfo setplayer avatar.......:");
                //set profile on playfab.......
                //pehele ye call kiya hua tha.......
                //SetPlayerAvatar((2).ToString());
            }
            if (result.AccountInfo.TitleInfo.DisplayName == null)
            {
                if (PlayerPrefs.GetString("Login_Type") == "Facebook")
                {
                    SetUserNameOnPlayFab();
                }
            }
        }, failUpdate);
    }

    [Header("Panel Where User Can Enter GroupUsername If It Does Not Exist")]
    /// <summary>
    /// Reference to EnterUserNamePanel panel 
    /// </summary>
    public GameObject EnterUserNamePanel;
    /// <summary>
    /// Condition check if player has a username or not, And allow to apply based on that state.
    /// </summary>
    public void ToOpenEnterGroupUserNameOrNot()
    {
        Debug.Log("Group Name Exists Or Not" + CheckIfGroupUserNameAlreadyExists());

        if (CheckIfGroupUserNameAlreadyExists())
        {
            EnterUserNamePanel.SetActive(false);
            ClanSystemController.Instance.ApplyToGroup();
        }
        else
        {
            EnterUserNamePanel.SetActive(true);
        }
    }

    public bool CheckIfGroupUserNameAlreadyExists()
    {
        //Group Name
        Debug.Log("Group Username is :" + currentMember.GetGroupUsername());
        return !(string.IsNullOrEmpty(currentMember.GetGroupUsername()));
    }
    /// <summary>
    ///  GetUserData successful call callback  
    /// </summary>
    /// <param name="dataResult">GetUserDataResult object</param>
    private void OnDataReceived(GetUserDataResult dataResult)
    {
        Dictionary<string, UserDataRecord>.KeyCollection keys = dataResult.Data.Keys;
        foreach (string key in keys)
        {
            Debug.LogError("Key: {0}" + key);
        }

        Debug.LogError("OnDataReceived:" + PlayerDataKey + ":contain che ke ny:" + dataResult.Data.ContainsKey(PlayerDataKey));
        if (dataResult.Data.ContainsKey(PlayerDataKey))
        {
            GetAccountInfo();
            currentMember.MemberPersonalData = JsonUtility.FromJson<MemberData>(dataResult.Data[PlayerDataKey].Value);
            EnergySystem.Instance.setplayerLevel(currentMember.MemberPersonalData.Level);
        }
        //LifeManager.Instance.StartCheckUserCloseWhilePlaying();
        Debug.LogError("OnDataReceived:" + GameDataKey + ":contain che ke ny:" + dataResult.Data.ContainsKey(GameDataKey));
        if (dataResult.Data.ContainsKey(GameDataKey))
        {
            //GetAccountInfo();
            Debug.LogError("updated data : " + dataResult.Data[GameDataKey].Value);
            Debug.LogError(sessionManagementString + " " + PlayerPrefs.GetString("Login_Type"));
        }
        
        //this method is used for cities and collection.......
        CitiesAndCollectiblesManager.Instance.LoginCall();
    }
    /// <summary>
    /// GetUserData failure call callback  
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    private void OnDataNotReceived(PlayFabError error)
    {
        Debug.Log(error);
    }
    /// <summary>
    /// Calls for LoginWithFacebook(). Based on the current state it links or logs player in.
    /// </summary>
    public void LinkWithFacebookOrLogin()
    {
        LoginWithFacebook();
    }
    /// <summary>
    /// Playfab LinkFacebookAccount api caller
    /// </summary>
    public void LinkWithFacebook()
    {
        LinkFacebookAccountRequest request = new LinkFacebookAccountRequest
        {
            AccessToken = PlayerPrefs.GetString("FacebookToken"),
            ForceLink = true
        };
        PlayFabClientAPI.LinkFacebookAccount(request, (resultCallback) =>
        {
            isFacebookLoggedIn = true;
            UnlinkDevices();
            OnFacebookLinkingSuccess();
        }, FailedlLinking);
    }
    /// <summary>
    /// PlayFabClient UnlinkDevice caller. Works for both ios and android.
    /// </summary>
    public void UnlinkDevices()
    {
#if UNITY_ANDROID || ANDROID
        UnlinkAndroidDeviceIDRequest request = new UnlinkAndroidDeviceIDRequest
        {
            AndroidDeviceId = ReturnDeviceId()
        };
        PlayFabClientAPI.UnlinkAndroidDeviceID(request, (resultCallback) =>
        {
            //if (PlayPabUiController.Instance.isFacebookLoginRequestFromSetting)
            //{
            //    PlayPabUiController.Instance.facebookLoginSuccessPopup.SetActive(true);
            //}
            //PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
            //getDataOnAccountLogin();
            //OnLoginCalls();
            Debug.Log(resultCallback.ToString());
        }, FailedlLinking);
#endif

#if UNITY_IOS || IOS
        UnlinkIOSDeviceIDRequest request = new UnlinkIOSDeviceIDRequest
        {
            DeviceId = ReturnDeviceId()
    };
    PlayFabClientAPI.UnlinkIOSDeviceID(request, (resultCallback) => {
            //if (PlayPabUiController.Instance.isFacebookLoginRequestFromSetting)
            //{
            //    PlayPabUiController.Instance.facebookLoginSuccessPopup.SetActive(true);
            //}
            //PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
            //getDataOnAccountLogin();
            //OnLoginCalls();
            Debug.Log(resultCallback.ToString());
        }, FailedlLinking);
#endif
    }
    /// <summary>
    /// Clear Player Data.
    /// </summary>
    public void ClearDataOnAndroidUserLoginWithFacebook()
    {
        currentMember.MemberRole = new Role("", "");
        currentMember.MemberGroup = new Group();
        ClanSystemController.Instance.playFabClanData = new PlayFabClanData();
        ClanSystemController.Instance.groupInfo = new NewGroupInformation();
        TradeSystemController.Instance.inventoryMessages = new InventoryMessages();
        TradeSystemController.Instance.lifeResponse = new LifeRequest();
        TradeSystemController.Instance.currencyResponse = new ClanSystem.VirtualCurrency.CloudResponse.ResponseContent();
        TradeSystemController.Instance.messageNumber = 0;
        TradeSystemController.Instance.awardResponse = new ClanSystem.AwardLife.CloudResponse.ResponseContent();
        TradeSystemController.Instance.consumeItemResponse = new ClanSystem.ConsumeItem.CloudResponse.ResponseContent();
        ChatManager.Instance.Unsubscribe();
        //PlayPabUiController.Instance.AfterFBLoginChangeScreen();
    }
    /// <summary>
    /// Playfab LoginWithFacebook api caller
    /// </summary>
    public void LoginWithFacebook()
    {
        Debug.LogError("LoginWithFacebook12345");
        if (!PlayerPrefs.HasKey("FacebookId"))
        {
            FaceBook_Login();
            return;
        }
        LoginWithFacebookRequest FBLoginRequest = new LoginWithFacebookRequest()
        {
            TitleId = PlayFabTitleId,
            CreateAccount = false,
            AccessToken = PlayerPrefs.GetString("FacebookToken")
        };
        PlayFabClientAPI.LoginWithFacebook(FBLoginRequest, (FBLoginResult) =>
        {
            ClearDataOnAndroidUserLoginWithFacebook();

            if (FBLoginResult.NewlyCreated)
            {
                first_login = true;
                PlayerPrefs.SetString("Login_Type", "Facebook");
                LinkFacebookAccountRequest FaceBookLinkRequest = new LinkFacebookAccountRequest()
                { AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString, ForceLink = true };
                PlayFabClientAPI.LinkFacebookAccount(FaceBookLinkRequest, result =>
                {
                    OnLoginSuccess(FBLoginResult);
                }, FailedlLinking);
            }
            else
            {
                Debug.LogError("FBLoginResult:" + FBLoginResult);
                OnLoginSuccess(FBLoginResult);
            }
            //ResetCurrentMember();
            FacebookManager.Instance.OnPlayfabFacebookAuthComplete();
            //chirag.......
            isFacebookLoggedIn = true;
            //FacebookManager.Instance.GetFbProfilePic(PlayerPrefs.GetString("FacebookToken"));
        }, (FBLoginError) =>
        {
            Debug.Log("Error logging in player with facebook ID: " + FBLoginError.ErrorMessage + "\n" + FBLoginError.ErrorDetails);
            Debug.Log(FBLoginError.Error);
            switch (FBLoginError.Error)
            {
                case PlayFabErrorCode.AccountNotFound:
                    {
                        LinkWithFacebook();
                    }
                    break;
                case PlayFabErrorCode.FacebookAPIError:
                    {
                        FaceBook_Login();
                    }
                    break;
            }
        });
    }
    /// <summary>
    /// Facebook failed linking callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void FailedlLinking(PlayFabError error)
    {
        Debug.Log(error);
    }
    /// <summary>
    /// Facebook Successfull linking callback
    /// </summary>
    /// <param name="result"></param>
    public void SuccessfulLinking(LinkFacebookAccountResult result)
    {
        first_login = true;
        Debug.Log(result);
    }

    #endregion
    #region Photon_Setup
    /// <summary>
    /// Playfab GetPhotonAuthenticationToken api caller
    /// </summary>
    public void GetPhotonDetails()
    {

        Debug.Log("COMING HERE TOO");
        GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonAppId

        };

        PlayFabClientAPI.GetPhotonAuthenticationToken(request,
            resultCallback =>
            {
                chatToken = resultCallback.PhotonCustomAuthenticationToken;
                Debug.Log("Starting Chat " + chatToken);
                ChatManager.Instance.AuthenticateWithPlayfab(chatToken);

            }, error =>
            {
                Debug.Log(error.Error);
            }

            );
    }
    #endregion
    #region Function_calls_Controllers

    public void RequestCurrency()
    {
        TradeSystemController.Instance.LoadVirtualCurrency();

        //chirag.......
        TradeSystemController.Instance.GetUserLifeMessages();
    }
    public void GetSpinWheelItems()
    {
        SpinWheelController.Instance.LoadSpinWheelTable();
        SpinWheelController.Instance.GetPremiumSpinWheel();
    }

    #endregion


    /// <summary>
    /// Call all the necassary functions to fetch the required resources
    /// </summary>
    public void OnLoginCalls()
    {
        Debug.LogError("faceBookLogin Success call thay che");

        //ChatScreenManager.instance.ClearFullChetScreen();

        //TournamentSystem.Instace.GetTeamTournamentForPlayer();
        //if (PlayPabUiController.Instance.IsStarTournamnetStarted != 0)
        //{
        //    TournamentSystem.Instace.GetPersonalTournamentEndTime();
        //}
      
        ClanSystemController.Instance.GetCurrentPlayerDataForGroup(PlayFabUserId);
        //UiController.Instance.OpenGroupOptions();
      
       // SpinWheelController.Instance.StopInvoke();
        RequestCurrency();
        GetSpinWheelItems();
        //InfiniteLivesController.Instance.CheckIfBonusLifeExpired();
        StoreSystemController.Instance.getIAPItems();
        StoreSystemController.Instance.CheckIfBonusLifeExpired();
        StoreSystemController.Instance.getVipTime();
        StoreSystemController.Instance.RegisterUserLoginTimeStamp();
        EventControllerScript.Instance.GetTodayEventDetails();
        EventControllerScript.Instance.GetDailyEventEndTime();
        TournamentSystem.Instace.getPersonalTournament();
        TournamentSystem.Instace.GetTeamEvent();
        EnergySystem.Instance.GetLevels();
        EnergySystem.Instance.ResumeTimer();
        InboxSystemController.Instance.OnSignin();
        GetPlayerCountry();
        StartCoroutine(FakePlayerHandler.Instance.GetLeaderboard());

        //this method are used to get all leaderboard.......
        LeaderboardSystem.Instance.GetLocalLeaderboard();
        LeaderboardSystem.Instance.GetLeaderboardTop100();
        LeaderboardSystem.Instance.GetLeaderboardOfFriends();
        ArenaAndMatchMakingBridge.Instance.GetAllCitiesData();
        ArenaAndMatchMakingBridge.Instance.GetAllGeneralsData();
        ArenaAndMatchMakingBridge.Instance.GetLevelsToBeFiltered();
        StartCoroutine(PlayerLevelsDataManager.Instance.GetPlayerAllStacks());

        //this method are used for piggy bank.......
        BankController.Instance.LoginCall();
        //this method is used for cities and collection.......
        //CitiesAndCollectiblesManager.Instance.LoginCall();
        //this line are used to Daily Reward.......umesh
        //ClanSystemController.Instance.ListMembershipRequestForCurrentMember();
        //PlayFabManager.Instance.GetPhotonDetails();

        //this method are used to get current utc time.......
        //Debug.LogError("GetTime 33333333333333333333333333.......");

        //this method are used to get latest version and check force update or not.......
      
    }
    public void OnFacebookLinkingSuccess()
    {
        //Functionality here
    }

   
    public void ResetCurrentMember()
    {
        currentMember = null;
    }
}

[System.Serializable]
public class VersionController
{
    public string LatestVersionAndroid;
}