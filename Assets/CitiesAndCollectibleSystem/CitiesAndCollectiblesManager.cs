using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
/// <summary>
/// Cities and collectibles manager class
/// </summary>
public class CitiesAndCollectiblesManager : MonoBehaviour
{
    /// <summary>
    /// Static instance of CitiesAndCollectiblesManager class
    /// </summary>
    public static CitiesAndCollectiblesManager Instance;
    /// <summary>
    /// Object of CitiesContainerClass. Data fetched from server will be assigned here
    /// </summary>
    /// <returns></returns>
    public CitiesContainerClass citiesContainerClassObject = new CitiesContainerClass();
    /// <summary>
    /// CityData object.  .Data fetched from server will be assigned here.
    /// </summary>
    public CityData receivedCityFromPlayer;
    /// <summary>
    /// CitiesNames object. All cities names data will be assigned to this object
    /// </summary>
    /// <returns></returns>
    public CitiesNames allCitiesNames = new CitiesNames();
    /// <summary>
    /// CitiesNames object. All cities names that player has visited data will be assigned to this object
    /// </summary>
    /// <returns></returns>
    public CitiesNames allCitiesNamesPlayerHasVisited = new CitiesNames();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [ContextMenu("GetAllCities")]
    /// <summary>
    /// Starts GetAllCitiesDownloader coroutine  
    /// </summary>
    void GetAllCities()
    {
        StartCoroutine(GetAllCitiesDownloader());

    }

    /// <summary>
    /// Downloads all cities data with interval of 0.20 second interval. Then adds them to all cities data list
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetAllCitiesDownloader()
    {
        for (int i = 0; i < allCitiesNames.allCitiesNames.Count; i++)
        {
            yield return new WaitForSeconds(0.20f);
            GetCityFromPlayer(allCitiesNames.allCitiesNames[i]);
        }
    }
    /// <summary>
    /// To be called on login to get cities data. Can also be called to reset
    /// </summary>
    public void LoginCall()
    {// apde e bool 1 kraviye eni niche aa call kray devay
        //if (PlayFabManager.Instance.currentMember.MemberPersonalData.Level >= GameManager.Instance.enableCityAndCollectableLevel)
        // Debug.LogError("Collection LoginCall:" + GameManager.Instance.PrologueLevel + ":bool:" + CollectionHandler.Instance.IsDefaultLevelReward);
        // if (GameManager.Instance.PrologueLevel >= 1 || CollectionHandler.Instance.IsDefaultLevelReward)
        // {
            GetAllAvailableCitiesNames();
        //}
    }
    /// <summary>
    /// Get city data for passed city
    /// </summary>
    /// <param name="cityName">Name of the city for which data is required</param>
    public void GetCityData(string cityName)
    {
        GetCityFromPlayer(cityName);
    }
    /// <summary>
    /// Update player data for the city
    /// </summary>
    /// <param name="cityName">Name of the city to update</param>
    /// <param name="c_data">Updated city data</param>
    public void UpdateCityData(string cityName, CityData c_data)
    {
        UpdateCityFromPlayer(cityName, c_data);
    }

    [ContextMenu("GetAllAvailableCitiesNames")]
    /// <summary>
    /// Get All possible cities names from the server 
    /// </summary>
    void GetAllAvailableCitiesNames()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetAllAvailableCitiesNames",
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    [ContextMenu("GetAllCitiesNamesPlayerHasVisited")]
    /// <summary>
    /// Get names of all cities player has ever visited. 
    /// This method and its data member can be used for unlocked
    /// and locked cities
    /// </summary>
    void GetAllCitiesNamesPlayerHasVisited()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetAllCitiesNamesPlayerHasVisited",
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    [ContextMenu("AddAllCitiesNamePlayerHasVisited")]
    /// <summary>
    /// Add City name if player has played its levels 
    /// </summary>
    /// <param name="city"></param>
    void AddAllCitiesNamePlayerHasVisited(string city)
    {
        if (!allCitiesNamesPlayerHasVisited.allCitiesNames.Contains(city))
        {
            allCitiesNamesPlayerHasVisited.allCitiesNames.Add(city);
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
            {
                FunctionName = "AddAllCitiesNamePlayerHasVisited",
                FunctionParameter = new
                {
                    City = city,
                },
                GeneratePlayStreamEvent = true,
            }, CloudScriptSuccess, CloudScriptFailure);
        }
    }

    /// <summary>
    /// Get city data from player data. 
    /// Also if player has not visited that given city it will add that city data to player data.
    /// If player doesnt have this city data .
    /// Then it is recommended to call AddAllCitiesNamePlayerHasVisited("cityname") too.
    ///  So both data sets are synced 
    /// 
    /// </summary>
    /// <param name="city"></param>  
    void GetCityFromPlayer(string city)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetCityFromPlayer",
            FunctionParameter = new
            {
                City = city
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    [ContextMenu("UpdateCityFromPlayer")]
    /// <summary>
    /// Updated City Data. e.g. add cards in player data etc    /// </summary>
    /// <param name="city"></param>
    /// <param name="cityData"></param>
    void UpdateCityFromPlayer(string city, CityData cityData)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "UpdateCityFromPlayer",
            FunctionParameter = new
            {
                City = city,
                CityData = JsonUtility.ToJson(cityData)
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// CallBack on Cloud Scrpit function Execution Success to populate playfabclandata objects
    /// </summary>
    void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);
        switch (result.FunctionName)
        {
            case "GetCityFromPlayer":
                {
                    try
                    {
                        receivedCityFromPlayer = JsonUtility.FromJson<CityData>(result.FunctionResult.ToString());
                        AddCity(receivedCityFromPlayer);
                    }
                    catch (Exception e)
                    {
                        if (e != null)
                        {
                            Debug.Log("Wrong Response:" + e);
                        }
                    }
                }
                break;
            case "UpdateCityFromPlayer":
                {
                    //Success
                }
                break;
            case "GetAllAvailableCitiesNames":
                {
                    allCitiesNames = JsonUtility.FromJson<CitiesNames>("{\"allCitiesNames\":" + result.FunctionResult.ToString() + "}");
                    GetAllCities();
                }
                break;
            case "GetAllCitiesNamesPlayerHasVisited":
                {
                    try
                    {
                        allCitiesNamesPlayerHasVisited = JsonUtility.FromJson<CitiesNames>("{\"allCitiesNames\":" + result.FunctionResult.ToString() + "}");
                    }
                    catch
                    {
                        Debug.Log("Wrong Response");
                    }
                }
                break;
            case "AddAllCitiesNamePlayerHasVisited":
                {
                    //Success
                }
                break;
        }
    }
    /// <summary>
    /// CloudScript failed call callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    void CloudScriptFailure(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
/// <summary>
/// Add this city in player data
/// </summary>
/// <param name="cityData"></param>
    private void AddCity(CityData cityData)
    {
        Debug.LogError("addcity000000000000000000000000000000000000000000:");
        //this method is used to set home city info button and timer.......

        int index = citiesContainerClassObject.allCities.FindIndex(obj => obj.cityName == cityData.cityName);
        if (index == -1)
        {
            citiesContainerClassObject.allCities.Add(cityData);
        }
        else
        {
            citiesContainerClassObject.allCities[index] = cityData;
        }

        // for (int i = 0; i < CollectionHandler.Instance.datas.allCollectibles.allCities.Count; i++)
        // {
        //     if (CollectionHandler.Instance.datas.allCollectibles.allCities[i].cityName == cityData.cityName)
        //     {
        //         string levelGroup = CollectionHandler.Instance.datas.allCollectibles.allCities[i].cityLevelRange;
        //         string[] arr = levelGroup.Split('-');
        //         int startLevel = int.Parse(arr[0]);

        //         if (PlayFabManager.Instance.currentMember.MemberPersonalData.Level >= startLevel)
        //         {
        //             CollectionHandler.Instance.datas.allCollectibles.allCities[i].isActivated = true;
        //         }
        //         else
        //         {
        //             CollectionHandler.Instance.datas.allCollectibles.allCities[i].isActivated = false;
        //         }

        //         CollectionHandler.Instance.datas.allCollectibles.allCities[i].badgeStarAcquired = cityData.badgeStarAcquired;
        //         CollectionHandler.Instance.datas.allCollectibles.allCities[i].badgeStarRequired = cityData.badgeStarRequired;

        //         CollectionHandler.Instance.datas.allCollectibles.allCities[i].outfitStarAcquired = cityData.outfitStarAcquired;
        //         CollectionHandler.Instance.datas.allCollectibles.allCities[i].outfitStarRequired = cityData.outfitStarRequired;

        //         for (int j = 0; j < cityData.allCards.Count; j++)
        //         {
        //             CollectionHandler.Instance.datas.allCollectibles.allCities[i].allCards[j].starAcquired = cityData.allCards[j].starAcquired;
        //             CollectionHandler.Instance.datas.allCollectibles.allCities[i].allCards[j].starRequired = cityData.allCards[j].starRequired;
        //         }
        //     }
      //  }

        // if (citiesContainerClassObject.allCities.Count == 5)
        // {
        //     StartCoroutine(CollectionHandler.Instance.LoadCollectibleDataCoroutine());
        // }
    }
/// <summary>
/// Used for reseting. Delete city data from player data
/// </summary>
/// <param name="city">City name to delete value against</param>
    void RemoveCityFromPlayer(string city)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "RemoveCityFromPlayer",
            FunctionParameter = new
            {
                City = city
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }
/// <summary>
/// Reset all progress
/// </summary>
/// <returns></returns>
    public IEnumerator ResetData()
    {
        citiesContainerClassObject.allCities.Clear();
        receivedCityFromPlayer = null;
        for (int i = 0; i < allCitiesNames.allCitiesNames.Count; i++)
        {
            yield return new WaitForSeconds(0.20f);
            RemoveCityFromPlayer(allCitiesNames.allCitiesNames[i]);
        }
        yield return new WaitForSeconds(2f);
        LoginCall();
    }
}