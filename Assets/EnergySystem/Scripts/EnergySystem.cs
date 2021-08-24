using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
public class EnergySystem : MonoBehaviour
{

    public static EnergySystem Instance;
    [SerializeField]
    internal LevelsData levelsData;
    public string timeToDisplay;
    [SerializeField]
    TimeSpan timeDifference;
    [SerializeField]
    TimeSpan secondDiff = new TimeSpan(0, 0, 1);
    [SerializeField] int playerLevel = 1;



    public void setplayerLevel(int lvl) {

        playerLevel = lvl;

    }
    public int getplayerLevel() {

        return playerLevel;
    }



    internal void ResumeTimer() // resume timer aafter logging in
    {
        //AddPlayerEnergyBasedOnTimerStart
        
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "GetEnergyUpdateTime"


            };

            PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptSuccess, CloudScriptFailure);

        


    }
    [ContextMenu("start timer")]
    public void StartTimer(int time=180)
    {
        if (IsInvoking(nameof(EnergyTimer)))
            return;

        timeToDisplay = "00:00";
        DateTime startTime = DateTime.UtcNow;
        DateTime endTime = DateTime.UtcNow.AddSeconds(time);
        timeDifference = (endTime - startTime);
        InvokeRepeating(nameof(EnergyTimer), 1, 1);

    }


    public void EnergyTimer()
    {
        timeDifference = timeDifference.Subtract(secondDiff);
        timeToDisplay = timeDifference.ToString();

        if (timeDifference.TotalSeconds < 1)
        {
            TradeSystemController.Instance.LoadVirtualCurrency();
            CancelInvoke();
            ResumeTimer();
        }


    }
    public bool CheckIfPlayerCanSpendEnergyAccordingToTheLevel()
    {
        return ( PlayFabManager.Instance.getEnergies() >= 
            levelsData.PlayerLevelsSetup.Find((obj) => obj.Level 
            == PlayFabManager.Instance.currentMember.GetLevelInt() ).Consumption);
    }

    public void Awake() 
    {

        Instance = this;
       // Starttimer();
    }
    
    public void GetLevels()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetLevels"
        };

        PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptSuccess, CloudScriptFailure);

    }

    public void deductEnergy()
    {

        if (CheckIfPlayerCanSpendEnergyAccordingToTheLevel())
        {
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "deductEnergy"
                
            
            };

            PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptSuccess, CloudScriptFailure);
     
        }
    
    }

  
    public void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {

        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);

        switch (result.FunctionName)
        {



            case "GetLevels":
                {

                    levelsData = JsonUtility.FromJson<LevelsData>(result.FunctionResult.ToString());
                 


                }
                break;

            case "deductEnergy":
                {
                    PlayFabManager.Instance.setEnergies(PlayFabManager.Instance.getEnergies() - 1);
                    TradeSystemController.Instance.LoadVirtualCurrency();

                }
                break;

            case "GetEnergyUpdateTime": 
                {
                    //StartTimer(int.Parse( result.FunctionResult.ToString() ) );
                }
                break;
        
        }

    }


    public void CloudScriptFailure(PlayFabError error)
    {

        Debug.Log(error.Error);
    }


}
