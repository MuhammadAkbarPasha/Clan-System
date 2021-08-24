using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
public class CurrencyController : MonoBehaviour
{
    // Start is called before the first frame update
    public static CurrencyController Instance;

    public void Awake() 
    {

        Instance = this;
    
    
    }
    internal void DeductHints()
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "deductHints"
        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    
    }

    internal void DeductCoins( int coinsToDeduct )
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "deductCoins"
        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    }
    internal void DeductDarts(int coinsToDeduct)
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "deductDarts"
        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    }

    internal void DeductLife()
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "deductLife"
        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    }

    internal void DeductTimer()
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "deductTimer"
        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    }


    private void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
    }

 /// <summary>
    /// CloudScript failed call callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void OnFailedResponse(PlayFabError error)
    {

        Debug.Log(error.Error);
    }

}
