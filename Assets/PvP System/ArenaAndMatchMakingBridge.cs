using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayFab.ClientModels;
using PlayFab;
[System.Serializable]
public class PlayerData
{
    public string PlayerArena;
}


[System.Serializable]
public class Arena
{
    public string arenaName;
    public int lowerLevel;
    public int upperLevel;

}


/// <summary>
/// This class is the bridge between Playfab and Multiplayer
/// </summary>
public class ArenaAndMatchMakingBridge : MonoBehaviour
{
    public static ArenaAndMatchMakingBridge Instance;
    public Text DebugText;

    public RootAllCities rootAllCities;
    public AllGenerals rootAllGenerals;
    public LevelsToBeFilteredClass levelsToBeFiltered;

    public void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
        Debug.Log("data here" + result.CustomData);


        switch (result.FunctionName)
        {
            case "GetAllCitiesData":
                {
                    if (result.FunctionResult == null)
                    {
                        Debug.Log("No Cities Data from Server");
                    }
                    else
                    {
                        Debug.Log("All Cities Data: " + result.FunctionResult.ToString());
                        rootAllCities = JsonUtility.FromJson<RootAllCities>(result.FunctionResult.ToString());
                    }
                }
                break;

            case "GetGeneralsData":
                {
                    if (result.FunctionResult == null)
                    {
                        Debug.Log("No Generals Data from Server");
                    }
                    else
                    {
                        Debug.Log("All Generals Data: " + result.FunctionResult.ToString());
                        rootAllGenerals = JsonUtility.FromJson<AllGenerals>(result.FunctionResult.ToString());
                    }
                }
                break;
            case "GetLevelsToBeFiltered":
                {
                    if (result.FunctionResult == null)
                    {

                        Debug.Log("No LevelsToBeFiltered Data from Server");
                    }
                    else
                    {
                        Debug.Log("All LevelsToBeFiltered Data: " + result.FunctionResult.ToString());
                        levelsToBeFiltered = JsonUtility.FromJson<LevelsToBeFilteredClass>(result.FunctionResult.ToString());

                    }


                }
                break;
        }
    }
    public void OnFailedResponse(PlayFabError error)
    {
        Debug.Log(error.Error);
    }

    [ContextMenu("UpdateStackProbablities")]
    public void UpdateStackProbablities()
    {
        int ProbToBeAdded = 0;
        bool General1Empty = false;
        bool General2Empty = false;
        bool General2Exists = false;
        int IndexOfGeneral1 = -1;
        int IndexOfGeneral2 = -1;

        Debug.Log("1ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
        foreach (CityClass city in rootAllCities.AllCities)
        {
            Debug.Log("EnterList:" + city.GeneralTypesIncluded[0].GeneralName);

            IndexOfGeneral1 = PlayerLevelsDataManager.Instance.generalStackSystem.stacks.FindIndex(e => e.name == city.GeneralTypesIncluded[0].GeneralName);

            if (IndexOfGeneral1 >= 0)
            {
                if (PlayerLevelsDataManager.Instance.generalStackSystem.stacks[IndexOfGeneral1].LevelsUnPlayed.Count == 0 && city.GeneralTypesIncluded[0].Probability != 0)
                {
                    ProbToBeAdded += city.GeneralTypesIncluded[0].Probability;
                    city.GeneralTypesIncluded[0].Probability = 0;
                    General1Empty = true;
                    Debug.Log("2ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
                }
                if (city.GeneralTypesIncluded.Count != 1)
                {
                    General2Exists = true;
                    IndexOfGeneral2 = PlayerLevelsDataManager.Instance.generalStackSystem.stacks.FindIndex(e => e.name == city.GeneralTypesIncluded[1].GeneralName);
                    if (IndexOfGeneral2 >= 0)
                    {
                        if (PlayerLevelsDataManager.Instance.generalStackSystem.stacks[IndexOfGeneral2].LevelsUnPlayed.Count == 0 && city.GeneralTypesIncluded[1].Probability != 0)
                        {
                            ProbToBeAdded += city.GeneralTypesIncluded[1].Probability;
                            city.GeneralTypesIncluded[1].Probability = 0;
                            General2Empty = true;
                            Debug.Log("3ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
                        }
                    }
                }
                Debug.Log("Should be here");


                if (General1Empty && !General2Empty && General2Exists)
                {
                    city.GeneralTypesIncluded[1].Probability += ProbToBeAdded;
                    Debug.Log("4ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
                }
                if (General2Empty && !General1Empty)
                {
                    city.GeneralTypesIncluded[0].Probability += ProbToBeAdded;
                    Debug.Log("5ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
                }
                if ((General1Empty && General2Empty) || !General2Exists)
                {
                    city.Probability += ProbToBeAdded;
                    Debug.Log("6ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
                }
                ProbToBeAdded = 0;
                General1Empty = false;
                General2Empty = false;
                General2Exists = false;
                Debug.Log("7ProbTobeAdded:" + ProbToBeAdded + ",General1Empty:" + General1Empty + ",General2Empty:" + General2Empty);
            }
        }
    }
    public void GetAllCitiesData()
    {
        ExecuteCloudScriptRequest GetAllCitiesDataContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetAllCitiesData"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetAllCitiesDataContainer, OnSuccessfulResponse, OnFailedResponse);
    }
    public void GetAllGeneralsData()
    {
        ExecuteCloudScriptRequest GetGeneralsDataContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetGeneralsData"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetGeneralsDataContainer, OnSuccessfulResponse, OnFailedResponse);
    }
    public void GetLevelsToBeFiltered()
    {
        ExecuteCloudScriptRequest GetLevelsToBeFilteredContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetLevelsToBeFiltered"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetLevelsToBeFilteredContainer, OnSuccessfulResponse, OnFailedResponse);
    }
    private CityClass GetArena(int levelNumber)
    {
        // Arena Functionality Here 
        CityClass arena = rootAllCities.AllCities.Find((ob) => (ob.MinRange <= levelNumber && ob.MaxRange >= levelNumber));
        arena = arena == null ? rootAllCities.AllCities[0] : arena;
        Debug.LogError("Arena" + arena);
        return arena;
        // Arena Functionality Here 
    }
    Dictionary<string, CityClass> ArenasDict = new Dictionary<string, CityClass>();

    public int GetRandomLevel(string arenaName)
    {
        Debug.LogError("arena name : " + arenaName);
        CityClass arena = ArenasDict[arenaName];
        return UnityEngine.Random.Range(arena.MinRange, arena.MaxRange + 1);
    }
    public List<string> GetGeneralList(string cityName)
    {
        Debug.Log("Akbar Test 1 City Name" + cityName);

        Generals = rootAllCities.AllCities.Find((ob) => ob.CityName == cityName).GeneralTypesIncluded.Select((obj) => obj.GeneralName).ToList();

        Debug.Log("Akbar Test 2 Generals Name" + JsonUtility.ToJson(Generals));
        return Generals;

    }
    public string DebugCityName;
    public List<string> Generals;
    [ContextMenu("Test Here")]
    public void Test()
    {
        Debug.Log("TT" + JsonUtility.ToJson(GetGeneralList(DebugCityName)));



    }
    public string GetArenaName(int currentPlayerLevel)
    {
        return GetArena(currentPlayerLevel).CityName;
    }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }

    }

    public void Start()
    {
        foreach (CityClass arena in rootAllCities.AllCities)
        {
            ArenasDict.Add(arena.CityName, arena);
        }
    }
    public void PrintDebugOnNextLine(string line)
    {
        DebugText.text += "\n" + line;
    }
}
