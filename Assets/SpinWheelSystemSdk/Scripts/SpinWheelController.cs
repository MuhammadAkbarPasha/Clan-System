using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
public class SpinWheelController : MonoBehaviour
{
    /// <summary>
    /// Static instance of SpinWheelController class
    /// </summary>
    public static SpinWheelController Instance;
    /// <summary>
    /// SpinWheel object. This is where SpinWheel data from the server will be assigned. 
    /// </summary>
    /// <returns></returns>
    public CloudResponse.SpinWheel.SpinWheel spinWheel = new CloudResponse.SpinWheel.SpinWheel();
    public void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// Get prize table data against the prize table name 
    /// </summary>
    /// <param name="spinWheelName">SpinWheel or PrizeTable name to fetch table fetch for</param>
    internal void LoadSpinWheelTable(string spinWheelName = "SpinWheel1")
    {
        ExecuteCloudScriptRequest loadSpinWheelTableRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "getSpinWheelInfo",
            FunctionParameter = new
            {
                spinWheelName = spinWheelName
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(loadSpinWheelTableRequest, OnSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    /// Ask the playfab server to evaluate the spin wheel and fetch the response
    /// </summary>
    /// <param name="spinWheelName">SpinWheel or PrizeTable name to evaluate</param>
    internal void EvaluateSpinWheel(string spinWheelName = "SpinWheel1")
    {
        ExecuteCloudScriptRequest evaluateSpinWheelTableRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "evaluateSpinWheel",
            FunctionParameter = new
            {
                spinWheelName = spinWheelName
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(evaluateSpinWheelTableRequest, OnSuccessfulResponse, OnFailedResponse);
    }


    /// <summary>
    /// Get special spinWheel table
    /// </summary>
    public void GetPremiumSpinWheel()
    {
        ExecuteCloudScriptRequest loadSpinWheelTableRequest = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetServerData",
            FunctionParameter = new
            {
                keys = "CurrentPremiumSpinWheel"
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(loadSpinWheelTableRequest, resultCallback =>
        {
            Debug.Log("Premium Spin Wheel Here" + resultCallback.FunctionResult.ToString());
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultCallback.FunctionResult.ToString());
            string currentPremiumSpinWheel = values["CurrentPremiumSpinWheel"];

            LoadSpinWheelTable(currentPremiumSpinWheel);

        }, OnFailedResponse);
    }



    /// <summary>
    /// CallBack on Cloud Scrpit function Execution Success to populate playfabclandata objects
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    private void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
        if (result.FunctionName == "getSpinWheelInfo")
        {
            spinWheel = new CloudResponse.SpinWheel.SpinWheel();
            spinWheel = JsonUtility.FromJson<CloudResponse.SpinWheel.SpinWheel>(result.FunctionResult.ToString());
        }
        if (result.FunctionName == "evaluateSpinWheel")
        {

            TradeSystemController.Instance.LoadVirtualCurrency();
        }
    }
    /// <summary>
    /// Cloud script function failure callback
    /// </summary>
    /// <param name="error">PlayFabError object containing error information </param>
    public void OnFailedResponse(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
}
