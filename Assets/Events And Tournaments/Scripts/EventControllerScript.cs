using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;

/// <summary>
/// Player event management class
/// </summary>
public class EventControllerScript : MonoBehaviour
{
/// <summary>
/// Current ongoing objective object
/// </summary>
    public Objective objective;
    /// <summary>
    /// EventControllerScript static instance 
    /// </summary>
    public static EventControllerScript Instance;

    [SerializeField]
    [Header("Time In Seconds Before Daily Event Ends")]
    /// <summary>
    /// Remaining time of the current event
    /// </summary>
    int dailyEventEndTimeInSeconds;
    public void Awake()
    {
        Instance = this;

    }

    /// <summary>
    /// Get DailyEvent Time In Seconds
    /// </summary>
    /// 
    [ContextMenu("GetDailyEventEndTime")]
    public void GetDailyEventEndTime()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetDailyEventEndTime",

        }, CloudScriptSuccess, CloudScriptFailure);
    }
/// <summary>
/// Get current player event data 
/// </summary>
    public void GetTodayEventDetails()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetTodayEventDetails",
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }
    /// <summary>
    /// Let server know that current on going objective has been completed
    /// </summary>
    public void CompletedSingleObjective()  // increment signle objective 
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "CompletedSingleObjective",
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }
    /// <summary>
    /// Complete all the objective of the current event has been achieved. For test purposes 
    /// </summary>
    public void CompletedAllObjectives() //increment full objectives 
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "CompletedAllObjectives",
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }




 /// <summary>
    /// callback on successful response from the server
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    public void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {

        Debug.Log(result.FunctionName);
        Debug.Log(result.FunctionResult);

        switch (result.FunctionName)
        {
            case "GetTodayEventDetails":
                {
                    if (result.FunctionResult == null)
                        return;
                    objective = JsonUtility.FromJson<Objective>(result.FunctionResult.ToString());
                    //chirag.......
                }
                break;
            case "CompletedSingleObjective":
                {
                    if (result.FunctionResult != null)
                    {
                        //Debug.LogError("CompletedSingleObjective.......");
                        objective = JsonUtility.FromJson<Objective>(result.FunctionResult.ToString());
                        //chirag.......
                    }
                }
                break;
            case "CompletedAllObjectives":
                {
                    if (result.FunctionResult != null)
                    {
                        objective = JsonUtility.FromJson<Objective>(result.FunctionResult.ToString());
                        //chirag.......
                    }
                }
                break;

            case "GetDailyEventEndTime":
                {
                    if (result.FunctionResult != null)
                    {
                        dailyEventEndTimeInSeconds = int.Parse(result.FunctionResult.ToString());

                        //chirag.......
                    }
                }
                break;

        }

    }
/// <summary>
/// 
/// </summary>
/// <param name="error"></param>
    public void CloudScriptFailure(PlayFabError error)
    {

        Debug.Log("Eventcontroller"+error.Error);
    }



}
