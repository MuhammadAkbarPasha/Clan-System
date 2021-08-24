using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;

[System.Serializable]

/// <summary>
/// Piggy bank data class
/// </summary>
public class PiggyBank
{
    public int lowerLimit;
    public int upperLimit;
    public bool isTimeDependent;
    public int currentCoins;
    public bool needToResetBankData;
    public string itemId;
    public bool isEligibleForPurchase = false;
}
[System.Serializable]
/// <summary>
/// Item in store
/// </summary>
public class ProductInStore
{
    public string offerTitle;
    public List<Items> items;
    public int originalPrice;
    public int offerPrice;
}
[System.Serializable]
/// <summary>
/// Item data class
/// </summary>
public class Items
{
    public string itemId;
    public string itemName;
    public int number;
}
[System.Serializable]
/// <summary>
/// Player coins class 
/// </summary>
public class PlayerCoins
{
    public int coins;
}

[System.Serializable]
/// <summary>
/// Piggy bank data class
/// </summary>
public class PiggyBankEvent
{
    public bool returnValue;
    public int remainingTime;
}

public class BankController : MonoBehaviour
{
    /// <summary>
    /// PiggyBank product id
    /// </summary>
    [SerializeField] string piggyBankId = "";
    /// <summary>
    /// static instance of BankController class
    /// </summary>
    public static BankController Instance;
    /// <summary>
    /// PiggyBank object. Refering to the current piggyBank instance details
    /// </summary>
    public PiggyBank piggyBank;
    /// <summary>
    /// PiggyBankEvent object. Refering to the current event in foucs
    /// </summary>
    public PiggyBankEvent piggyBankEvent;
    /// <summary>
    /// ProductInStore object. Refering to the current product in focus
    /// </summary>
    public ProductInStore productInStore;
    /// <summary>
    /// Current player coins data
    /// </summary>
    public PlayerCoins player;

    public GameObject bankPanel;
    public GameObject loadingObj;
    public GameObject piggyBankHomeObj;
    public Image piggyBankHomeFillImage;
    public Text infoText;

    public GameObject sparkImg;
    public Slider coinStorageBar;
    public Button claimButton;
    public TextMeshProUGUI currentCoinsText;
    public TextMeshProUGUI minCoinText;
    public TextMeshProUGUI maxCoinText;
    public float timerRemains;
    public List<TextMeshProUGUI> timerTexts;
    public int lastAddedPiggyBankCoins;
    public bool isFirstTimePiggyBankAddCoin = false;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    /// <summary>
    /// Function to call on login 
    /// </summary>
    public void LoginCall()
    {
        piggyBankEventTimeRemains();
        GetBankData();
    }

    /// <summary>
    ///  A function to add given coins in player's bank by calling
    /// private function of updating bank data.
    /// 
    /// </summary>
    /// <param name="numberOfCoins"></param>
    public void OnAddBankCoinsButtonClick(int numberOfCoins)
    {
        if (piggyBank.currentCoins == piggyBank.upperLimit)
        {
            Debug.Log("bank is already full");
        }
        else if (piggyBank.currentCoins < piggyBank.upperLimit) // is there space to add more coins ?
        {
            if ((piggyBank.currentCoins + numberOfCoins) >= piggyBank.upperLimit)
            {
                Debug.Log("bank is going to be full");
                var maxCoins = piggyBank.upperLimit - piggyBank.currentCoins;
                UpdateBankData(maxCoins, false);
            }
            else
            {
                Debug.Log("update bankdata after add coins");
                UpdateBankData(numberOfCoins, false);
            }
        }
        else
        {
            Debug.Log("cannot add more coins");
        }
    }

    /// <summary>
    ///  Just a test function to check the functionality of purchase.
    /// </summary>
    public void OnClaimedButtonClick()
    {
        StoreSystemController.Instance.OnClickPurchaseBtn(piggyBankId);
        //UpdateVCCoins();
    }

    /// <summary>
    ///  A test function to get value of coins that is set in store's custom data
    /// </summary>
    public void OnSetPiggyBankCoinsInStoreClick()
    {
        SetPiggyBankCoinsInLocal(piggyBank.currentCoins); // locally setting coins in store
    }

    /// <summary>
    ///  Function to call a private function of getting dynamic product
    /// </summary>
    public void OnGetDynamicProductClick()
    {
        GetDynamicProductInStore();
    }

    // ___________________________________ FUNCTIONS FOR BUTTONS (END) ________________________________________
    /// <summary>
    ///  A callback for GetUserInventory
    /// </summary>
    private void GetVirtualCurrency()
    {
        var requestInventory = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(requestInventory, OnVCSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    ///  On successful callback of GetVirtualCurrency
    /// </summary>
    /// <param name="result"></param>
    private void OnVCSuccessfulResponse(GetUserInventoryResult result)
    {
        result.VirtualCurrency.TryGetValue("CO", out player.coins);
    }

    /// <summary>
    ///  Call a handler on cloud to add given coins inplayer's virtual currency
    /// 
    /// </summary>
    public void UpdateVCCoins()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "AddCoins",
            FunctionParameter = new
            {
                quantity = piggyBank.currentCoins
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    ///  Test function to set local game objects
    /// </summary>
    /// <param name="coins"></param>
    private void SetPiggyBankCoinsInLocal(int coins)
    {
        if (productInStore.offerTitle == "PiggyBank") //check if the product is really piggybank
        {
            for (int i = 0; i < productInStore.items.Count; i++)
            {
                if (productInStore.items[i].itemId == "CO")
                {
                    productInStore.items[i].number = coins;
                }
            }
        }
        else
        {
            Debug.Log("product is not a piggybank");
        }
    }

    /// <summary>
    ///  Make cloud call to get dynamic product
    /// </summary>
    private void GetDynamicProductInStore()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetDynamicContentsProduct",
            FunctionParameter = new
            {
                Id = piggyBank.itemId
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    ///  Make cloud call to update player title bank data
    /// </summary>
    /// <param name="coinToAdd"></param>
    /// <param name="resetStatus"></param>
    private void UpdateBankData(int coinToAdd, bool resetStatus)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdatePiggyBankData",
            FunctionParameter = new
            {
                coins = coinToAdd
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    /// Make a cloud call to load bank data from shared group data.
    /// There is no need to call it again and again when player logged in,
    /// just call it when the player open piggybank panel (where purchase offers are given) 
    /// </summary>
    private void GetBankData()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetPiggyBankData"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    /// <summary>
    /// Make cloud call to check if bank event time remains or not
    /// </summary>
    private void piggyBankEventTimeRemains()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "piggyBankEventTimeRemains"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSuccessfulResponse, OnFailedResponse);
    }

    private void OnFailedResponse(PlayFabError error)
    {
        loadingObj.SetActive(false);
        Debug.Log(error.ErrorMessage);
    }

    /// <summary>
    /// All cloud function's sucess callbacks.
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);

        switch (result.FunctionName)
        {
            case "GetPiggyBankData":
                {
                    piggyBank = JsonUtility.FromJson<PiggyBank>(result.FunctionResult.ToString());
                    infoText.text = "current coins in bank = " + piggyBank.currentCoins;
                    coinStorageBar.maxValue = piggyBank.upperLimit;
                    coinStorageBar.value = piggyBank.currentCoins;
                    minCoinText.text = "" + piggyBank.lowerLimit;
                    maxCoinText.text = "" + piggyBank.upperLimit;
                    currentCoinsText.text = "" + piggyBank.currentCoins;
                    float fillAmount = (float)piggyBank.currentCoins / (float)piggyBank.upperLimit;
                    piggyBankHomeFillImage.fillAmount = fillAmount;
                    //for claimimg reward
                    if (piggyBank.currentCoins >= piggyBank.lowerLimit)
                    {
                        piggyBank.isEligibleForPurchase = true;
                        claimButton.interactable = true;
                        sparkImg.SetActive(true);
                    }
                    else
                    {
                        piggyBank.isEligibleForPurchase = false;
                        claimButton.interactable = false;
                        sparkImg.SetActive(false);
                    }
                    if (piggyBank.isTimeDependent)
                        piggyBankEventTimeRemains();

                    loadingObj.SetActive(false);

                    if (isFirstTimePiggyBankAddCoin)
                    {
                        isFirstTimePiggyBankAddCoin = false;
                        OnAddBankCoinsButtonClick(lastAddedPiggyBankCoins);
                    }
                }
                break;
            case "UpdatePiggyBankData":
                {
                    Debug.Log("Personal Bank Data Updated Successfully");
                    GetBankData();
                    GetVirtualCurrency();
                }
                break;
            case "piggyBankEventTimeRemains":
                {
                    piggyBankEvent = JsonUtility.FromJson<PiggyBankEvent>(result.FunctionResult.ToString());
                    Debug.Log("Remaining time in seconds is = " + piggyBankEvent.remainingTime);
                    piggyBankHomeObj.SetActive(true);
                    timerRemains = piggyBankEvent.remainingTime;
                    CancelInvoke(nameof(DeductTime));
                    InvokeRepeating(nameof(DeductTime), 0, 1);
                }
                break;
            case "GetDynamicContentsProduct":
                {
                    productInStore = JsonUtility.FromJson<ProductInStore>(result.FunctionResult.ToString());

                    if (result.FunctionResult.ToString() != "-1") // check if product exist
                    {
                        Debug.Log("Product found in store and values are stored");
                    }
                    else
                    {
                        Debug.Log("product not found in store");
                    }
                }
                break;
            case "AddCoins":
                {
                    Debug.Log("Virtual coins added");
                    bankPanel.SetActive(false);
                    UpdateBankData(-piggyBank.currentCoins, false);
                }
                break;
            default:
                break;
        }
    }

    void DeductTime()
    {
        if (timerRemains > 0)
        {
            timerRemains--;
            TimeSpan time = TimeSpan.FromSeconds(timerRemains);
            string hour = ((int)time.TotalHours).ToString();
            string minutes = ((int)time.Minutes).ToString();
            string second = ((int)time.Seconds).ToString();
            if (time.TotalHours < 10)
            {
                hour = "0" + ((int)time.TotalHours);
            }
            if (time.Minutes < 10)
            {
                minutes = "0" + (int)time.Minutes;
            }
            if (time.Seconds < 10)
            {
                second = "0" + (int)time.Seconds;
            }
            string str = hour + ":" + minutes + ":" + second;
            for (int i = 0; i < timerTexts.Count; i++)
            {
                timerTexts[i].text = str;
            }

            if (timerRemains <= 0)
            {
                CancelInvoke();
                piggyBankHomeObj.SetActive(false);
            }
        }
    }

}