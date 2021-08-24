using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using PlayFab;
using PlayFab.CloudScriptModels;

public class PlayerLevelsDataManager : MonoBehaviour
{



    public StackSystem cityStackSystem = new StackSystem();
    public StackSystem generalStackSystem = new StackSystem();
    public static PlayerLevelsDataManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void GetStacksFromServer()
    {


    }

    public void SetStacksOnServer()
    {


    }
    public void Start()
    {

        //   GetStackFromDataIfNoStackOnServer();

    }
    /* public void GetStackFromDataIfNoStackOnServer()
     {

         foreach (CityClass cityClass in ArenaAndMatchMakingBridge.Instance.AllCities)
         {
             cityStackSystem.stacks.Add(new StackClass(cityClass.CityName, "City", cityClass.MinRange, cityClass.MaxRange));
         }
         foreach (GeneralClass generalClass in ArenaAndMatchMakingBridge.Instance.AllGenerals)
         {
             generalStackSystem.stacks.Add(new StackClass(generalClass.GeneralName, "General", generalClass.minRange, generalClass.maxRange));
         }


     }*/





    public void CloudScriptFailure(PlayFabError error)
    {

        Debug.Log(error.Error);
    }





    public void PopLevelFromUnplayedStackAndPushInPlayedStack()
    {

        switch (PlayersScoreController.Instance.levelObject.levelType)
        {

            case "City":
                {

                    cityStackSystem.stacks.Find((ob) => ob.name == PlayersScoreController.
                    Instance.levelObject.StackName).LevelsUnPlayed.
                    Remove(PlayersScoreController.Instance.levelObject.levelNumber);

                    cityStackSystem.stacks.Find((ob) => ob.name == PlayersScoreController.Instance.levelObject.StackName).LevelsPlayed.Add(PlayersScoreController.Instance.levelObject.levelNumber);
                    PopLevelFromUnplayedStackAndPushInPlayedStack("CityStackSystem", PlayersScoreController.Instance.levelObject.StackName, PlayersScoreController.Instance.levelObject.levelNumber);
                }
                break;
            case "General":
                {
                    generalStackSystem.stacks.Find((ob) => ob.name == PlayersScoreController.Instance.levelObject.StackName).LevelsUnPlayed.Remove(PlayersScoreController.Instance.levelObject.levelNumber);
                    generalStackSystem.stacks.Find((ob) => ob.name == PlayersScoreController.Instance.levelObject.StackName).LevelsPlayed.Add(PlayersScoreController.Instance.levelObject.levelNumber);
                    PopLevelFromUnplayedStackAndPushInPlayedStack("GeneralStackSystem", PlayersScoreController.Instance.levelObject.StackName, PlayersScoreController.Instance.levelObject.levelNumber);
                }
                break;

        }

    }








    public void PopLevelFromUnplayedStackAndPushInPlayedStack(string StackName, string cityName, int levelNumber)
    {

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "PopLevelFromUnplayedStackAndPushInPlayedStack", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new
            {
                StackRequired = StackName,
                CityName = cityName,
                LevelNumber = levelNumber

            },
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, result =>
        {
            Debug.Log(result.FunctionResult);

        }
       , CloudScriptFailure);
    }





    [ContextMenu("Test")]
    public void AddNewStackSystem(string StackName, string stackData)
    {

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "AddNewStackSystem", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new
            {
                StackRequired = StackName,
                Data = stackData

            },
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, result =>
        {
            Debug.Log("AddNewStackSystem" + result.FunctionResult);

        }
       , CloudScriptFailure);
    }

    public IEnumerator GetPlayerAllStacks()
    {

        yield return new WaitForSeconds(10f);
        GetStackSystem("CityStackSystem");
        GetStackSystem("GeneralStackSystem");
    }



    [ContextMenu("Test1")]
    public void GetStackSystem(string StackName)
    {

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "GetStackSystem", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new
            {
                StackRequired = StackName

            },
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, (System.Action<ExecuteCloudScriptResult>)(result =>
        {
            Debug.Log("GetStackSystem Result :" + result.FunctionResult);
            Debug.Log("GetStackSystem stackName : " + StackName);
            switch (StackName)
            {
                case "CityStackSystem":
                    {
                        if (result.FunctionResult == null) AddDefaultStack(ArenaAndMatchMakingBridge.Instance.rootAllCities.AllCities);
                        else
                        {
                            JsonUtility.FromJsonOverwrite(result.FunctionResult.ToString(), cityStackSystem);
                            if(cityStackSystem.stacks.Count <   1) AddDefaultStack(ArenaAndMatchMakingBridge.Instance.rootAllCities.AllCities);
                        }
                    }
                    break;
                case "GeneralStackSystem":
                    {
                        if (result.FunctionResult == null) AddDefaultStack(ArenaAndMatchMakingBridge.Instance.rootAllGenerals.Generals);
                        else 
                        {
                            JsonUtility.FromJsonOverwrite(result.FunctionResult.ToString(), generalStackSystem);
                            if (generalStackSystem.stacks.Count < 1) AddDefaultStack(ArenaAndMatchMakingBridge.Instance.rootAllGenerals.Generals);
                        }
                    }
                    break;

            }

        })
       , this.CloudScriptFailure);
    }

    public void AddDefaultStack(List<GeneralClass> Generals)
    {
        foreach (GeneralClass generalClass in Generals)
        {
            generalStackSystem.stacks.Add(new StackClass(generalClass.GeneralName, "General", generalClass.minRange, generalClass.maxRange));
        }
        AddNewStackSystem("GeneralStackSystem", JsonUtility.ToJson(generalStackSystem));
    }
    public void AddDefaultStack(List<CityClass> Cities)
    {
        foreach (CityClass cityClass in Cities)
        {
            cityStackSystem.stacks.Add(new StackClass(cityClass.CityName, "City", cityClass.MinRange, cityClass.MaxRange));
        }
        AddNewStackSystem("CityStackSystem", JsonUtility.ToJson(cityStackSystem));
    }

    public void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {

        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);

        switch (result.FunctionName)
        {
            case "AddNewStackSystem":
                {

                }
                break;

        }
    }
}
