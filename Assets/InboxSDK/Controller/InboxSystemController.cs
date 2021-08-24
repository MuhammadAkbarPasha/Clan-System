using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab.ClientModels;
using PlayFab;

public class InboxSystemController : MonoBehaviour
{

    /// <summary>
    /// InboxSystemController static instance
    /// </summary>
    public static InboxSystemController Instance;
    /// <summary>
    /// Server response will be passed to this object
    /// </summary>
    [SerializeField] GiftsResponse inboxResponse;
    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);

        }
    }
    /// <summary>
    /// Signin Callback
    /// </summary>
    public void OnSignin()
    {
        InvokeRepeating(nameof(CheckForCoinsGift), 5, 30);


    }
    /// <summary>
    /// CheckIfPlayerHasGottenAnyGift
    /// </summary>
    public void CheckForCoinsGift()
    {
        Debug.Log("Inbox Working Or Not");
        ExecuteCloudScriptRequest removeUserLifeMessage = new ExecuteCloudScriptRequest
        {
            FunctionName = nameof(CheckForCoinsGift)
        };

        PlayFabClientAPI.ExecuteCloudScript(removeUserLifeMessage, CloudScriptSuccess, CloudScriptFailure);

    }
    /// <summary>
    /// Opens Gift And Consume The Reward against the instanceId And Call LoadVirtuallCurrecnyInTheResponse
    /// </summary>
    public void OpenCoinsGift(string instanceId)
    {
        ExecuteCloudScriptRequest removeUserLifeMessage = new ExecuteCloudScriptRequest
        {
            FunctionName = "OpenCoinsGift",
            FunctionParameter = new
            {
                InstanceId = instanceId
            },
        };
        PlayFabClientAPI.ExecuteCloudScript(removeUserLifeMessage, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// CloudScript successful call callback  
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult Object</param>
    public void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {

        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);
        switch (result.FunctionName)
        {
            case nameof(CheckForCoinsGift):
                {
                    if (result.FunctionResult != null)
                    {
                        inboxResponse = JsonUtility.FromJson<GiftsResponse>("{\"Gifts\":" + result.FunctionResult.ToString() + "}");
                        //PlayPabUiController.Instance.RefreshInbox(inboxResponse);
                    }
                }
                break;
            case "OpenCoinsGift":
                {
                    //chirag.......
                    // PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
                    // if (!PlayPabUiController.Instance.InboxItemWithoutReward)
                    // {
                    //     PlayPabUiController.Instance.MailBoxPopupCloseButtonClick();
                    // }
                    // else
                    // {
                    //     PlayPabUiController.Instance.InboxItemWithoutReward = false;
                    // }
                    // PlayPabUiController.Instance.openMailInboxItemScript.GiveRewardItem();                    

                    CheckForCoinsGift();
                    result.FunctionName = "loadVirtualCurrency";
                    TradeSystemController.Instance.OnSuccessfulResponse(result);
                }
                break;
        }
    }
    /// <summary>
    /// On sign out callback
    /// </summary>
    public void SignOut()
    {

        CancelInvoke();
    }
    /// <summary>
    /// CloudScript failed call callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void CloudScriptFailure(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
}
