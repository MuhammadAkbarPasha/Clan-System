using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class StoreSystemController : MonoBehaviour, IStoreListener
{
    /// <summary>
    /// IStoreController object used for controlling the purchases
    /// </summary>
    static IStoreController m_StoreController;

    [SerializeField]
    /// <summary>
    /// Playfab Store items
    /// </summary>
    /// <returns></returns>
    StoreSystem.StoreResponse storeResponse = new StoreSystem.StoreResponse();
    /// <summary>
    /// Static refernce of StoreSystemController
    /// </summary>
    public static StoreSystemController Instance;
    /// <summary>
    /// Used to control the flow of this script
    /// </summary>
    public bool IsInitialized;

    [SerializeField]
    /// <summary>
    /// Claim VIP reward button
    /// </summary>
    Button claimbutton;
    /// <summary>
    /// Products purchase button prefab
    /// </summary>
    public GameObject iapButton;
    /// <summary>
    /// Products purchase buttons parent
    /// </summary>
    public GameObject iapButtonParent;


    void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// Helper function for purchasing Product 
    /// </summary>
    /// <param name="productId">Product to purchase</param>
    public void OnClickPurchaseBtn(string productId)
    {
        Debug.LogError("onclickpurchasebtn:" + productId + ":IsInitialized:" + IsInitialized);
        if (!IsInitialized)
        {
            Debug.LogError("IAP Not Initialized");
            return;
        }
        m_StoreController.InitiatePurchase(productId);
    }
    /// <summary>
    /// Start purchase of the passed product
    /// </summary>
    /// <param name="productId">Product Id to purchase</param>
    void BuyProductID(string productId)
    {
        if (!IsInitialized) Debug.LogError("IAP Not Initialized");
        m_StoreController.InitiatePurchase(productId);
    }
    /// <summary>
    /// Instantiate button for the each item and add BuyProductID function as listener
    /// </summary>
    public void PopulateIAPButtons()
    {
        if (!IsInitialized)
        {
            Debug.Log("not initialized so returned");
            return;
        }
        foreach (var item in storeResponse.Store)
        {
            GameObject gb = Instantiate(iapButton, iapButtonParent.transform);
            gb.transform.Find("Text").gameObject.GetComponent<Text>().text = "Buy " + item.DisplayName;
            gb.GetComponent<Button>().onClick.AddListener(() =>
            {

                BuyProductID(item.ItemId);
            });
            gb.name = item.ItemId;
        }
    }
    /// <summary>
    /// Playfab GetCatalogItems api caller. Used to get all the items in the store
    /// </summary>
    private void GetContainers()
    {

        GetCatalogItemsRequest req = new GetCatalogItemsRequest
        {

            CatalogVersion = storeResponse.CatalogVersion

        };

        PlayFabClientAPI.GetCatalogItems(req, resultCallback =>
        {

            if (resultCallback.Catalog.Count < 1)
                return;
            foreach (StoreSystem.Store st in storeResponse.Store)
            {

                if (resultCallback.Catalog.Exists((obj) => obj.ItemId == st.ItemId))
                    st.DisplayName = resultCallback.Catalog.Find((obj) => obj.ItemId == st.ItemId).DisplayName;
            }

            InitializePurchasing();
        }, OnFailedResponse);
    }

/// <summary>
/// Initialize seasonal products
/// </summary>
/// <returns></returns>    
    public IEnumerator InitializeSeasonalProductsForPurchase()
    {
        if (SeasonalOffers.Instance != null)
        {
            HashSet<ProductDefinition> itemsHashSet = new HashSet<ProductDefinition>();

            foreach (var item in SeasonalOffers.Instance.StoreResponse.Store)
            {

                itemsHashSet.Add(new ProductDefinition(item.ItemId, ProductType.Consumable));

                Debug.Log("coming here to setup" + item.ItemId);
            }

            while (m_StoreController == null)
                yield return new WaitForSeconds(1f);
            m_StoreController.FetchAdditionalProducts(itemsHashSet, OnProductsFetched, OnInitializeFailed);

        }
        //   CreateNewButtonsForDiscountedProducts();
    }
    /// <summary>
    /// On fetched Callback
    /// </summary>
    public void OnProductsFetched()
    {
        //Populate Buttons Here
    }

    /// <summary>
    /// callback on successful response from the server
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    private void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {


        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
        switch (result.FunctionName)
        {
            case "getStoreItems":
                {
                    if (result.FunctionResult == null)
                        return;
                    storeResponse = JsonUtility.FromJson<StoreSystem.StoreResponse>(result.FunctionResult.ToString());
                    GetContainers();
                }
                break;
            case "purchase150Coins":
                {
                    Debug.Log(result.FunctionResult);
                }
                break;
            case "CheckIfExpired":
                {
                    PlayFabManager.Instance.bonusLifeTimer = JsonUtility.FromJson<LifeTimer>(result.FunctionResult.ToString());
                    PlayFabManager.Instance.Bonus_Life_timer_init();
                }
                break;


            case "ExpireBonusLife":
                {
                    PlayFabManager.Instance.bonusLifeTimer = JsonUtility.FromJson<LifeTimer>(result.FunctionResult.ToString());
                    PlayFabManager.Instance.Bonus_Life_timer_init();


                }
                break;

            case "deductLife":
                {

                    TradeSystemController.Instance.LoadVirtualCurrency();

                }
                break;

        }
    }


    /// <summary>
    /// Callback on failed response from the server
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void OnFailedResponse(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
    [ContextMenu("GetItems")]
    /// <summary>
    /// getStoreItems CloudScript function caller 
    /// </summary>
    public void getIAPItems()
    {
        DiscountedProductsManager.Instance.GetLimitedOfferItems();
        //SeasonalOfferScript.Instance.getSeasonalItems();
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getStoreItems"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }
    /// <summary>
    ///  Initialize Unity IAP system
    /// </summary>
    public void InitializePurchasing()
    {
        IsInitialized = true;
        ConfigurationBuilder builder;

        if (Application.platform == RuntimePlatform.Android)
        {
            //builder = ConfigurationBuilder.Instance(Google.Play.Billing.GooglePlayStoreModule.Instance());
            builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
        }
        else
        {
            builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
        }
        //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
        foreach (var item in storeResponse.Store)
        {
            builder.AddProduct(item.ItemId, ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this, builder);

    }


    /// <summary>
    /// successful initialization callback
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
    }

    /// <summary>
    /// Failed initialization callback
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }
    /// <summary>
    /// Failed purchase callback
    /// </summary>
    /// <param name="product"></param>
    /// <param name="failureReason"></param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
    /// <summary>
    /// A purchase succeeded.
    /// </summary>
    /// <param name="purchaseEvent"> The <c>PurchaseEventArgs</c> for the purchase event. </param>
    /// <returns> The result of the succesful purchase </returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (!IsInitialized)
        {
            return PurchaseProcessingResult.Complete;
        }
        if (e.purchasedProduct == null)
        {
            Debug.LogWarning("Attempted to process purchase with unknown product. Ignoring");
            return PurchaseProcessingResult.Complete;
        }
        if (string.IsNullOrEmpty(e.purchasedProduct.receipt))
        {
            Debug.LogWarning("Attempted to process purchase with no receipt: ignoring");
            return PurchaseProcessingResult.Complete;
        }
        Debug.Log("Processing transaction: " + e.purchasedProduct.transactionID);
#if UNITY_ANDROID
        var googleReceipt = StoreSystem.GooglePurchase.FromJson(e.purchasedProduct.receipt);
        PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest()
        {
            CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
            PurchasePrice = (uint)(e.purchasedProduct.metadata.localizedPrice * 100),
            ReceiptJson = googleReceipt.PayloadData.json,
            Signature = googleReceipt.PayloadData.signature
        }, result =>
        {
            Debug.Log("Validation successful!");
            Debug.Log("Product Id is " + e.purchasedProduct.definition.id);
            if (DiscountedProductsManager.Instance.products.discountedProducts.Exists((ob) => ob.realProduct == e.purchasedProduct.definition.id))
            {
                awardLimitedTimeOfferProduct(DiscountedProductsManager.Instance.products.discountedProducts.Find((ob) => ob.realProduct == e.purchasedProduct.definition.id).realProduct);
            }
            else
            {
                awardDynamicProduct(e.purchasedProduct.definition.id);
            }
        },
           error =>
           {
               Debug.LogError("StoreManagerU Validation failed");
               Debug.Log("Validation failed: " + error.GenerateErrorReport());
#if UNITY_EDITOR

               Debug.Log("Product Id is " + e.purchasedProduct.definition.id);

               if (DiscountedProductsManager.Instance.products.discountedProducts.Exists((ob) => ob.realProduct == e.purchasedProduct.definition.id))
               {
                   awardLimitedTimeOfferProduct(DiscountedProductsManager.Instance.products.discountedProducts.Find((ob) => ob.realProduct == e.purchasedProduct.definition.id).realProduct);
               }
               else
               {
                   awardDynamicProduct(e.purchasedProduct.definition.id);
               }
#endif
               //chirag.......
               //purchaseFailedPopup.SetActive(true);
           }
        );
#endif
#if UNITY_IOS
        StoreSystem.Receipt receiptiOS = JsonUtility.FromJson<StoreSystem.Receipt>(e.purchasedProduct.receipt);
        string receiptPayload = receiptiOS.Payload;
        PlayFabClientAPI.ValidateIOSReceipt(new ValidateIOSReceiptRequest()
        {
            CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
            PurchasePrice = (int)(e.purchasedProduct.metadata.localizedPrice * 100),
            ReceiptData = receiptPayload
        }, result =>
        {
            Debug.Log("Validation successful!");
            Debug.Log("Product Id is " + e.purchasedProduct.definition.id);

            if (IspurchaseFromLevel)
            {
                string currentLevelString = GameManager.Instance.GetLevelString(PlayFabManager.Instance.currentMember.MemberPersonalData.Level);
                GameAnalytics.NewBusinessEvent(e.purchasedProduct.metadata.isoCurrencyCode, Convert.ToInt32(e.purchasedProduct.metadata.localizedPrice * 100)
                                                , "Purchased on level "+ currentLevelString + " Shop item product id", e.purchasedProduct.definition.id, "");

                GameAnalytics.NewDesignEvent("Level" + currentLevelString + " Shop item product id " + e.purchasedProduct.definition.id, (float)e.purchasedProduct.metadata.localizedPrice * 100);
            }
            else
            {
                GameAnalytics.NewBusinessEvent(e.purchasedProduct.metadata.isoCurrencyCode, Convert.ToInt32(e.purchasedProduct.metadata.localizedPrice * 100)
                                                , "Shop item product id", e.purchasedProduct.definition.id, "");

                GameAnalytics.NewDesignEvent("Shop item product id " + e.purchasedProduct.definition.id, (float)e.purchasedProduct.metadata.localizedPrice * 100);
            }
           
            if (DiscountedProductsManager.Instance.products.discountedProducts.Exists((ob) => ob.realProduct == e.purchasedProduct.definition.id))
            {
                awardLimitedTimeOfferProduct(DiscountedProductsManager.Instance.products.discountedProducts.Find((ob) => ob.realProduct == e.purchasedProduct.definition.id).realProduct);
            }
            else
            {
                awardDynamicProduct(e.purchasedProduct.definition.id);
            }
        },
          error =>
          {
              Debug.Log("Validation failed: " + error.GenerateErrorReport());
#if UNITY_EDITOR
               Debug.Log("Product Id is " + e.purchasedProduct.definition.id);
               
                GameAnalytics.NewBusinessEvent(e.purchasedProduct.metadata.isoCurrencyCode, Convert.ToInt32(e.purchasedProduct.metadata.localizedPrice * 100)
                                               , "Shop item product id ", e.purchasedProduct.definition.id, "");

               string currentLevelString = GameManager.Instance.GetLevelString(PlayFabManager.Instance.currentMember.MemberPersonalData.Level);
               GameAnalytics.NewDesignEvent("Level" + currentLevelString + " Shop item product id "+ e.purchasedProduct.definition.id, (float)e.purchasedProduct.metadata.localizedPrice * 100);

            if (DiscountedProductsManager.Instance.products.discountedProducts.Exists((ob) => ob.realProduct == e.purchasedProduct.definition.id))
            {
                awardLimitedTimeOfferProduct(DiscountedProductsManager.Instance.products.discountedProducts.Find((ob) => ob.realProduct == e.purchasedProduct.definition.id).realProduct);
            }
            else
            {
                awardDynamicProduct(e.purchasedProduct.definition.id);
            }
#endif
            //chirag.......
            PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
            purchaseFailedPopup.SetActive(true);
          }
       );
#endif
        return PurchaseProcessingResult.Complete;
    }
    /// <summary>
    /// Award player with the limited time offer product. One of the special products.
    /// </summary>
    /// <param name="product"></param>
    private void awardLimitedTimeOfferProduct(string product)
    {
        Debug.Log("In awardLimitedTimeOfferProduct" + product);
        Debug.Log("Real Product Is" + product);
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "awardLimitedTimeOfferProduct",
            FunctionParameter = new
            {
                productId = product
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, resultCallback =>
        {
            TradeSystemController.Instance.LoadVirtualCurrency();

        }, OnFailedResponse);
    }
    #region InApps_functions
    /// <summary>
    /// Call awardDynamicProduct CloudScript function. This will award the player with the prdouct over the playfab Server
    /// </summary>
    /// <param name="product"></param>
    private void awardDynamicProduct(string product)
    {
        Debug.Log("Real Product Is" + product);
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "awardDynamicProduct",
            FunctionParameter = new
            {
                productId = product
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, resultCallback =>
        {
            // Success Call of getting the product
            ///////////        
            ////////////////////////////////
            /// This is Where LoadVirtualCurrency Should be called. Because 
            Debug.Log(resultCallback.CustomData);
        }, OnFailedResponse);
    }

    /// <summary>
    /// Expire this player BonusLifeWhen time completed
    /// </summary>
    public void ExpireBonusLife()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "ExpireBonusLife"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);


    }
    /// <summary>
    /// Check to see the status of the BonusLife timer
    /// </summary>
    public void CheckIfBonusLifeExpired()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "CheckIfExpired"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);


    }

    #endregion
    #region VIP_InApp_Functions
    /// <summary>
    /// Save Player Login Time
    /// </summary>
    public void RegisterUserLoginTimeStamp()
    {

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "RegisterUserLoginTimeStamp"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);


    }
    /// <summary>
    /// Fetch this player vip remaining time
    /// </summary>
    public void getVipTime()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getVipTime"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, resultCallback =>
        {
            Debug.Log(resultCallback.FunctionResult.ToString());
            PlayFabManager.Instance.vipTimer = JsonUtility.FromJson<LifeTimer>(resultCallback.FunctionResult.ToString());
            claimbutton.gameObject.SetActive(PlayFabManager.Instance.vipTimer.useBonusLife);
        }, OnFailedResponse);
    }

    public IEnumerator InitializeLimitedOfferProductsForPurchase()
    {
        if (DiscountedProductsManager.Instance != null)
        {
            HashSet<ProductDefinition> itemsHashSet = new HashSet<ProductDefinition>();

            foreach (var item in DiscountedProductsManager.Instance.StoreResponse.Store)
            {

                itemsHashSet.Add(new ProductDefinition(item.ItemId, ProductType.Consumable));

                Debug.Log("coming here to setup" + item.ItemId);
            }

            while (m_StoreController == null)
                yield return new WaitForSeconds(0.1f);

        }
        //   CreateNewButtonsForDiscountedProducts();
    }
    #endregion
}

