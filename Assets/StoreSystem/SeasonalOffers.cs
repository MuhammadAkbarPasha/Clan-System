using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System;
using StoreSystem;
public class SeasonalOffers : MonoBehaviour
{

    public static SeasonalOffers  Instance;
    [SerializeField]
    private StoreSystem.StoreResponse storeResponse = new StoreSystem.StoreResponse();
    public StoreSystem.StoreResponse StoreResponse { get => storeResponse; set => storeResponse = value; }


    
    public GameObject SeasonalOfferProductParent;
    public GameObject SeasonalOfferProductButton;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }


    public void getSeasonalItems()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getSeasonalItems"

        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }


    public void CreateButtonsForSeasonalProducts()
    {

        foreach (Store store in storeResponse.Store)
        {


            CustomData cd = store.CustomData;
            if (cd.timer > 0)
            {
                GameObject gb = Instantiate(SeasonalOfferProductButton, SeasonalOfferProductParent.transform);
                gb.GetComponent<LimitedTimeOfferProduct>().SetUi(cd.items, cd.originalPrice, cd.offerPrice);
                gb.GetComponent<LimitedTimeOfferProduct>().StartTimer(cd.timer);
                gb.GetComponent<LimitedTimeOfferProduct>().SetUpButtonOnClick(store.ItemId);
            }

        }





    }



    private void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {


        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
        switch (result.FunctionName)
        {
            case "getSeasonalItems":
                {
                    if (result.FunctionResult == null)
                        return;
                    storeResponse = JsonUtility.FromJson<StoreSystem.StoreResponse>(result.FunctionResult.ToString());
                    StartCoroutine(StoreSystemController.Instance.InitializeSeasonalProductsForPurchase());
                    CreateButtonsForSeasonalProducts();
                }
                break;

        }
    }


    public void OnFailedResponse(PlayFabError error)
    {

        Debug.Log(error.Error);
    }




}
