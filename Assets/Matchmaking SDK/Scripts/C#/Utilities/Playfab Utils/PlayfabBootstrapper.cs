using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
public class PlayfabBootstrapper : MonoBehaviour
{
    public bool loggedIn;
    [ContextMenu("Login")]
    private void Start()
    {
        loggedIn = false;
        var request = new LoginWithCustomIDRequest() { CustomId = "customId",TitleId= "35D4F", CreateAccount=true };
        request.CustomId = PlayFabSettings .DeviceUniqueIdentifier;
        PlayFabClientAPI.LoginWithCustomID(request, (result)=> { loggedIn = true; Debug.Log("Player successfully loggedin!" + result); },
            error=> { Debug.LogError("Something went wrong!" + error); });
    }
}
