using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreSystem;
using System.Linq;
using PlayFab.ClientModels;
using PlayFab;
using System;
public class DiscountedProductsManager : MonoBehaviour
{

    /// <summary>
    /// Static instance of DiscountedProductsManager class
    /// </summary>
    public static DiscountedProductsManager Instance;
    /// <summary>
    /// Store data from Playfab will be assigned here after being fetched
    /// </summary>
    /// <returns></returns>
    [SerializeField] StoreSystem.StoreResponse storeResponse = new StoreSystem.StoreResponse();
    /// <summary>
    /// Setter and Getter of storeResponse
    /// </summary>
    /// <value></value>
    public StoreSystem.StoreResponse StoreResponse { get => storeResponse; set => storeResponse = value; }
    /// <summary>
    /// Discounted products fetched from the Playfab
    /// </summary>
    public DiscountedProducts products;
    /// <summary>
    /// One second time difference 
    /// </summary>
    /// <returns></returns>
    TimeSpan secondDiff = new TimeSpan(0, 0, 1);
    /// <summary>
    /// Parent panel in which limited time offer products will be instantiated
    /// </summary>
    [SerializeField] GameObject LimitedTimerOfferProductParent;
    /// <summary>
    /// Limited time offer product prefab object
    /// </summary>
    [SerializeField] GameObject LimitedTimerOfferProductButton;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    #region API's
    /// <summary>
    /// Playfab API getLimitedOfferItems caller. API returns limited offer items
    /// </summary>
    public void GetLimitedOfferItems()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getLimitedOfferItems"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    [ContextMenu("Get Discounted Products")]
    /// <summary>
    /// Playfab API GetLimitedTimeOfferProductsForPlayers caller. API returns limited products for the player
    /// </summary>
    public void GetDiscountedProducts()
    {
        CancelInvoke(nameof(AllProductsTimer));

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetLimitedTimeOfferProductsForPlayers"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    /// Match products with in app purchases
    /// </summary>
    private void MapProductsWithInApps()
    {
        foreach (DiscountedProduct discountedProduct in products.discountedProducts)
        {
            if (discountedProduct.isEligible)
            {
                CreateNewButtonsForDiscountedProduct(discountedProduct);
            }
        }
    }



    /// <summary>
    /// Make the current player eligible for discounted product. 
    /// This happens based on level passed. Eligibilty means that player will be able to fetch that product.
    /// </summary>
    /// <param name="currentLevel">Level for which </param>
    public void MakePlayerEligibleForThisDiscountedProduct(int currentLevel)
    {
        Debug.LogError("makeeligible  " + currentLevel);
        CancelInvoke(nameof(AllProductsTimer));
        // DiscountedProduct discountedProduct = products.discountedProducts[productId];
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "MakePlayerEligibleForThisDiscountedProduct",
            FunctionParameter = new
            {
                Level = currentLevel
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, resultCallback =>
        {
            products = JsonUtility.FromJson<DiscountedProducts>("{\"discountedProducts\":" + resultCallback.FunctionResult.ToString() + "}");
            MapProductsWithInApps();
            AllProductsTimerInit();
            StartCoroutine(StoreSystemController.Instance.InitializeLimitedOfferProductsForPurchase());
            //CreateNewButtonsForDiscountedProduct(discountedProduct);
        }, OnFailedResponse);
    }

    /// <summary>
    /// CloudScript successful call callback  
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult Object</param>
    private void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
        switch (result.FunctionName)
        {
            case "GetLimitedTimeOfferProductsForPlayers":
                {

                    products = JsonUtility.FromJson<DiscountedProducts>("{\"discountedProducts\":" + result.FunctionResult.ToString() + "}");
                    MapProductsWithInApps();
                    AllProductsTimerInit();
                }
                break;
            case "getLimitedOfferItems":
                {
                    if (result.FunctionResult == null)
                        return;
                    storeResponse = JsonUtility.FromJson<StoreSystem.StoreResponse>(result.FunctionResult.ToString());
                    StartCoroutine(StoreSystemController.Instance.InitializeLimitedOfferProductsForPurchase());
                }
                break;

        }
    }


    /// <summary>
    /// CloudScript failed call callback
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void OnFailedResponse(PlayFabError error)
    {

        Debug.Log(error.Error);
    }
    #endregion
    #region UI And Time Control
    /// <summary>
    /// Start timer for all products
    /// </summary>
    public void AllProductsTimerInit()


    {
        foreach (DiscountedProduct discountedProduct in products.discountedProducts)
        {
            if (discountedProduct.isEligible)
            {
                try
                {
                    discountedProduct.startTime_ts = Convert.ToDateTime(discountedProduct.startTime);
                    discountedProduct.endTime_ts = Convert.ToDateTime(discountedProduct.endTime);
                    discountedProduct.timeLeft_ts = (discountedProduct.endTime_ts - discountedProduct.startTime_ts);
                    discountedProduct.timeLeft = discountedProduct.timeLeft_ts.ToString();
                }
                catch
                {

                    Debug.Log("Issues With Format Of Time, Do not call to make product eligible if there is no data in products");


                }
            }

        }

        InvokeRepeating(nameof(AllProductsTimer), 1, 1);

    }
    /// <summary>
    /// Subtract time and check if the product has experied and reset based on that 
    /// </summary>
    public void AllProductsTimer()
    {
        products.discountedProducts.Select((obj) =>
        {
            if (obj.isEligible)
            {
                obj.timeLeft_ts = obj.timeLeft_ts.Subtract(secondDiff);
                obj.timeLeft = obj.timeLeft_ts.ToString();
            }
            return obj;
        }).ToList();
        if (products.discountedProducts.Exists((obj) => obj.isEligible && obj.isRestart && obj.timeLeft_ts.TotalSeconds <= 0))
        {
            StartCoroutine(CallForResetProductsAfterWait());
        }

    }


    /// <summary>
    /// Fetch discounted products 
    /// </summary>
    /// <returns></returns>
    public IEnumerator CallForResetProductsAfterWait()
    {
        yield return new WaitForSeconds(2f);
        GetDiscountedProducts();

    }
    private void OnDisable()
    {
        CancelInvoke(nameof(AllProductsTimer));
    }


    /// <summary>
    /// Create buttons for the pased discounted product
    /// </summary>
    /// <param name="product">product for which button needs to be created</param>
    public void CreateNewButtonsForDiscountedProduct(DiscountedProduct product)
    {
        if (product.isEligible)
        {
            if (storeResponse.Store.Exists((ob) => ob.ItemId == product.realProduct))
            {
                CustomData cd = storeResponse.Store.Find((ob) => ob.ItemId == product.realProduct).CustomData;
                GameObject gb = Instantiate(LimitedTimerOfferProductButton, LimitedTimerOfferProductParent.transform);
                gb.GetComponent<LimitedTimeOfferProduct>().SetUi(product.levelRequired.ToString(), cd.items, cd.originalPrice, cd.offerPrice);
                gb.GetComponent<LimitedTimeOfferProduct>().SetUpButtonOnClick(product.realProduct);

            }
            return;
        }
        else
            return;

    }

    #endregion

}
