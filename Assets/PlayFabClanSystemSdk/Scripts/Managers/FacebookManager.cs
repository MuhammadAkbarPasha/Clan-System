using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Facebook.MiniJSON;
using PlayFab;
using PlayFab.ClientModels;
using System.IO;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager Instance;
    [HideInInspector] public string MyUniqueFbId = null;
    public string MyFbName = null;
    public string MyFbAvatarUrl = null;
    public bool IsloggedIn = false;
    [HideInInspector] public Sprite MyFbAvatar = null;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject, 0.2f);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.TitleId = "EF20A";
        }

        if (!FB.IsInitialized)
        {
            FB.Init(InitializationCallBack, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnPlayfabFacebookAuthComplete()
    {
        if (FB.IsLoggedIn)
        {
            FB.API("/me?fields=picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
            {
                if (result.Error == null)
                {
                    Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                    if (reqResult == null) Debug.Log("JEST NULL"); else Debug.Log("nie null");
                    MyFbAvatarUrl = ((reqResult["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;
                    ArenaAndMatchMakingBridge.Instance.PrintDebugOnNextLine(MyFbAvatarUrl);
                    StartCoroutine(DownloadAvatar());
                }
                else
                {
                    Debug.Log("Error retreiving image: " + result.Error);
                    ArenaAndMatchMakingBridge.Instance.PrintDebugOnNextLine("Error retreiving image: " + result.Error);
                }
            });
        }
    }

    public Sprite PlayerAvatar;
    public IEnumerator DownloadAvatar()
    {
        Debug.LogError("FacebookManager DownloadAvatar0000.......:" + MyFbAvatarUrl);
        if (Uri.IsWellFormedUriString(MyFbAvatarUrl, UriKind.Absolute))
        {
            WWW www = new WWW(MyFbAvatarUrl);
            yield return www;
            PlayerAvatar = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
            // Debug.LogError("DownloadAvtar1");
         //   PlayPabUiController.Instance.DisplayMyFbProfilePic(PlayerAvatar);
            //UIBattleLobby.Instance.SetPlayerSprite(PlayerAvatar);
        }
        else
        {
            Debug.LogError("PlayfabManager DownloadAvatar11111.......:" + MyFbAvatarUrl);
            PlayerAvatar = Sprite.Create(null, new Rect(0, 0, 200, 200), new Vector2(0.5f, 0.5f), 32);  // you win need to add your own functionality here for default avatars
                                                                                                        // Debug.LogError("DownloadAvtar2");
           // PlayPabUiController.Instance.DisplayMyFbProfilePic(PlayerAvatar);
            //UIBattleLobby.Instance.SetPlayerSprite(PlayerAvatar);
        }
    }

    void InitializationCallBack()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
            //	InitializeSession ();
        }
        else
        {
            Debug.Log("Facebook Initialization Failed");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 2;
        }
    }

    void InitializeSession()
    {
        string LoginType = PlayerPrefs.GetString("Login_Type");
        if (LoginType.Equals("Facebook"))
        {
            MyUniqueFbId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
            IsloggedIn = true;
            GetFacebookUserName();
            GetFacebookProfilePic(MyUniqueFbId);
            Debug.Log(MyUniqueFbId + " Is fb user ID");
            //	Debug.Log(MyUniqueFbId);
            //chirag
            // PlayPabUiController.Instance.isFacebookLoggedIn = true;
            // EditProfileScript.Instance.isFbLogfinWithSetting = true;
            // PlayPabUiController.Instance.settingFacebookButtonText.text = "Sign out from Facebook";
            // //GetFbProfilePic();
        }
    }

    public void getMyProfilePicture(string userID)
    {
        FB.API("/me?fields=picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Error == null)
            {
                Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

                if (reqResult == null) Debug.Log("JEST NULL"); else Debug.Log("nie null");

                MyFbAvatarUrl = ((reqResult["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;
                Debug.Log("My avatar " + MyFbAvatarUrl);
                StartCoroutine(PlayFabManager.Instance.UploadPlayerProfilePictureAsAvatar(MyFbAvatarUrl));
                StartCoroutine(loadImageMy(MyFbAvatarUrl));
            }
            else
            {
                Debug.Log("Error retreiving image: " + result.Error);
            }
        });
    }

    public IEnumerator loadImageMy(string url)
    {
        Debug.LogError("DownloadAvtar3");
        WWW www = new WWW(url);
        yield return www;
     //   PlayPabUiController.Instance.DisplayMyFbProfilePic(Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32));
    }

    void GetFacebookUserName()
    {
        FB.API("me?fields=first_name,last_name", Facebook.Unity.HttpMethod.GET, FbUserNameCallBack);
    }

    void FbUserNameCallBack(IResult ServerResponse)
    {
        Debug.LogError("server responce:+" + ServerResponse);
        MyFbName = ServerResponse.ResultDictionary["first_name"].ToString() + " " + ServerResponse.ResultDictionary["last_name"].ToString();

        if (PlayFabManager.Instance.username.Equals(""))
        {
            PlayFabManager.Instance.setUsername(MyFbName);
        }
        Debug.Log("Facebook name " + MyFbName);

        //chirag.......
        //PlayFabManager.Instance.SetUserNameOnPlayFab(MyFbName);
    }

    void GetFacebookProfilePic(string FbUserId)
    {
        StartCoroutine(DownloadPlayerAvatar("nope"));
    }

    //chirag.......
    #region download profile pic from facebook.......
    public void GetFbProfilePic(string userId)
    {
        StartCoroutine(DownloadImageFromFBUrl(userId));
    }
    public IEnumerator DownloadImageFromFBUrl(string userId)
    {
        Debug.LogError("DownLoadFromServer facebook:" + MyUniqueFbId);
        string url = "https" + "://graph.facebook.com/" + MyUniqueFbId + "/picture?type=large";
        WWW download = new WWW(url);
        while (!download.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        if (download.texture != null)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/" + "ProfileImages"))
                Directory.CreateDirectory(Application.persistentDataPath + "/" + "ProfileImages");

            var path = Path.Combine(Application.persistentDataPath + "/ProfileImages/fbProfile.png");

            File.WriteAllBytes(path, download.bytes);
            yield return new WaitForSeconds(1);
            // if (EditProfileScript.Instance.fbPicSp == null)
            // {
            //     Sprite fromTex = Sprite.Create(download.texture, new Rect(0, 0, download.texture.width, download.texture.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.FullRect);
            //     EditProfileScript.Instance.fbPicSp = fromTex;
            //     EditProfileScript.Instance.AddFbPicOnSpriteList();
            // }
            Debug.Log("Download done : " + path);
        }
    }

    //public Sprite GetSprite123(string path)
    //{
    //    byte[] pngBytes = File.ReadAllBytes(path);
    //    Texture2D tex = new Texture2D(2, 2);
    //    tex.LoadImage(pngBytes);
    //    tex.wrapMode = TextureWrapMode.Repeat;
    //    tex.filterMode = FilterMode.Bilinear;
    //    Sprite fromTex = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100f, 0, SpriteMeshType.FullRect);
    //    return fromTex;
    //}
    #endregion

    public void LogoutFromFacebook()
    {
        StartCoroutine(FBLogout());
    }

    IEnumerator FBLogout()
    {
        FB.LogOut();
        while (FB.IsLoggedIn)
        {
            print("Logging Out");

            yield return null;
        }
        print("Logout Successful");
        IsloggedIn = false;
        //chirag.......
        // PlayPabUiController.Instance.isFacebookLoggedIn = false;
        // PlayPabUiController.Instance.settingFacebookButtonText.text = "Sign in with Facebook";
        // PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
        // PlayPabUiController.Instance.facebookDisconnectedPopup.SetActive(true);

        // EditProfileScript.Instance.ResetData();

        //PlayFabManager.Instance.LinkWithFacebook();
        // File.Delete(EditProfileScript.Instance.FbPicFilePath);
        //PlayFabManager.Instance.SetUserNameOnPlayFab("");
    }

    IEnumerator DownloadPlayerAvatar(string AvatarURL)
    {
        yield return null;
        PlayerPrefs.SetString("FacebookToken", Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
        PlayerPrefs.SetString("FacebookId", Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
        getMyProfilePicture(Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
        PlayFabManager.Instance.LinkWithFacebookOrLogin();
    }

    public void UserWishesToLink()
    {
        //	SettingScript.Instance.userWishesToLink = true;
    }

    public void FBLogin()
    {
        StartCoroutine(WaitForFacebook());
    }

    public IEnumerator WaitForFacebook()
    {
        yield return new WaitForSeconds(3f);
        if (!IsloggedIn)
        {
            var perms = new List<string>() { "public_profile", "email" };//, "user_friends" 
            FB.LogInWithReadPermissions(perms, LoginResultCallback);
        }
        else
        {
            Debug.Log("Already Logged In");
            //chirag.......
         //   PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
        }
    }

    private void LoginResultCallback(ILoginResult Result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            MyUniqueFbId = aToken.UserId;
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            PlayerPrefs.SetString("Login_Type", "Facebook");
            InitializeSession();
        }
        else
        {
            Debug.Log("error cause" + Result.Error);
            Debug.Log("User cancelled login");
            //chirag.......
        //     PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
        //     PlayPabUiController.Instance.facebookLoginFailedPopup.SetActive(true);
        //
         }
    }
    #region LeaderBoard Api

    public void GetFacebookUserName(string id, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        FB.API("/" + id, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res))
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                Debug.Log(string.Format("FacebookManager.GetFacebookUserName => Success! (name: {0})",
                    res.ResultDictionary["name"]));

                if (successCallback != null)
                    successCallback(res);
            }));
    }

    public void GetFacebookUserPicture(string id, int width, int height, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}", id, height, width);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res) || res.Texture == null)
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);

                //Debug.Log("Leaderboard.GetFacebookUserPicture => Success!");
            }));
    }

    private bool ValidateResult(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            return true;

        //Debug.Log(string.Format("{0} is invalid (Cancelled={1}, Error={2}, JSON={3})",
        //    result.GetType(), result.Cancelled, result.Error, Facebook.MiniJSON.Json.Serialize(result.ResultDictionary)));

        return false;
    }
    #endregion

    public void destroy()
    {
        if (this.gameObject != null)
            DestroyImmediate(this.gameObject);
    }
}