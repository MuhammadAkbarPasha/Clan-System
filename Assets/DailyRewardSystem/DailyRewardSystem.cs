using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
/// <summary>
/// Daily Reward System controller class. Only the api calls and data classes for this system is made by us. 
/// </summary>
public class DailyRewardSystem : MonoBehaviour
{

    /// <summary>
    /// Statis instance to this class
    /// </summary>
    public static DailyRewardSystem Instance;
    public List<int> daysRewardInt;
    /// <summary>
    /// RewardData class object. Data received from the server will be assigned to it
    /// </summary>
    public RewardData rewardData;
    /// <summary>
    /// Data for daily reward will be assigned here
    /// </summary>
    public RewardCustomData dailyRewardCustomData;
    /// <summary>
    /// Data for days reward will be assigned here
    /// </summary>
    public RewardCustomData daysRewardCustomData;
    /// <summary>
    /// Server reward data will be assigned here
    /// </summary>
    public CustomDataServer CustomDatas;
    [Header("Daily Reward Panels")]
    public GameObject DailyRewardPopUp;
    public TextMeshProUGUI titleText;
    public GameObject thankYouPopUp;
    public List<DailyRewardItem> dailyRewardItems;
    public Image sliderImage;
    public TextMeshProUGUI dayCountText;
    public TextMeshProUGUI maxDaysText;
    public TextMeshProUGUI coinTextThankYou;
    [Header("Current Rewards")]
    public int coins = 0;
    public int life = 0;
    public int darts = 0;
    public int timers = 0;
    public int hints = 0;
    public string infiniteLifeStrDaily = "";
    public string infiniteLifeStrDays = "";

    [Header("Sprites")]
    public Sprite normalBgSprite;
    public Sprite collectBgSprite;

    public Color textBlueColor;

    public GameObject infoTurotialScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("DailyRewardData System Destroyed");
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Executes the function in the script after seconds passed
    /// </summary>
    /// <param name="FunctionName">Function to run</param>
    /// <param name="TimeInSeconds">How many seconds later to run</param>
    public void RunFunctionAfterGivenTimeInSeconds(string FunctionName, int TimeInSeconds)
    {
        Invoke(FunctionName, TimeInSeconds);
    }

    /// <summary>
    /// Call this function after sign to fetch DailyRewardSystem data from the server
    /// </summary>
    public void OnSignIn()
    {
        //Debug.LogError("Daily reward call thay che.......");
        ResetRewards();
        SetDailyRewardDataOnServer();
        GetRewardsCustomData();
    }
    /// <summary>
    /// callback on successful response from the server
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    public void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        //Debug.LogError("DailyReward Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult + "  " + result.FunctionName);
        Debug.Log("data here" + result.CustomData);

        switch (result.FunctionName)
        {
            case "SetDailyRewardData":
                {
                    if (result.FunctionResult == null)
                    {
                        rewardData = new RewardData();
                        SetDailyRewardDataOnServer();
                    }
                    else
                    {
                        ResetRewards();
                        rewardData = JsonUtility.FromJson<RewardData>(result.FunctionResult.ToString());
                        SetUI();
                        RunFunctionAfterGivenTimeInSeconds("SetDailyRewardDataOnServer", rewardData.SecondsLeftInNextDay);
                        GetDailyRewardData();
                        GetDaysRewardData();
                    }
                }
                break;
            case "GetDailyReward":
                {
                    if (result.FunctionResult == null)
                    {
                        Debug.Log("No such Reward Found");
                    }
                    else
                    {
                        dailyRewardCustomData = JsonUtility.FromJson<RewardCustomData>(result.FunctionResult.ToString());
                        TestRewardData(result.FunctionResult.ToString());
                        for (int i = 0; i < dailyRewardCustomData.items.Count; i++)
                        {
                            if (dailyRewardCustomData.items[i].itemName == "Coins")
                            {
                                // GameManager.TotalCoin += int.Parse(dailyRewardCustomData.items[i].itemId);
                                coins += dailyRewardCustomData.items[i].quantity;
                            }
                            else if (dailyRewardCustomData.items[i].itemName == "Darts")
                            {
                                // GameManager.TotalDirt += int.Parse(dailyRewardCustomData.items[i].itemId);
                                darts += dailyRewardCustomData.items[i].quantity;
                            }
                            else if (dailyRewardCustomData.items[i].itemName == "Hints")
                            {
                                // GameManager.TotalHint += int.Parse(dailyRewardCustomData.items[i].itemId);
                                hints += dailyRewardCustomData.items[i].quantity;
                            }
                            else if (dailyRewardCustomData.items[i].itemName == "Timers")
                            {
                                // GameManager.TotalTimer += int.Parse(dailyRewardCustomData.items[i].itemId);
                                timers += dailyRewardCustomData.items[i].quantity;
                            }
                            else if (dailyRewardCustomData.items[i].itemName == "Lives")
                            {
                                // GameManager.TotalLife += int.Parse(dailyRewardCustomData.items[i].itemId);
                                life += dailyRewardCustomData.items[i].quantity;
                            }
                            else if (dailyRewardCustomData.items[i].itemName == "Life")
                            {
                                if (dailyRewardCustomData.items[i].itemId == "L1/2")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDaily = "L1/2";
                                }
                                else if (dailyRewardCustomData.items[i].itemId == "L0")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDaily = "L0";
                                }
                                else if (dailyRewardCustomData.items[i].itemId == "L1")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDaily = "L1";
                                }
                                else if (dailyRewardCustomData.items[i].itemId == "L2")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDaily = "L2";
                                }
                                else if (dailyRewardCustomData.items[i].itemId == "L3")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDaily = "L3";
                                }
                            }
                        }
                        DailyRewardPopUp.SetActive(true);

                        // if (GameManager.Instance.dailyRewardInfoTutorial == 0)
                        // {
                        //     GameManager.Instance.dailyRewardInfoTutorial = 1;
                        //     GameManager.Instance.SaveUserData();
                        //     infoTurotialScreen.SetActive(true);
                        //     Invoke("FalseInfoTutorialScreen", 2f);
                        // }
                    }
                }
                break;
            case "GetDaysReward":
                {
                    if (result.FunctionResult == null)
                    {
                        Debug.Log("No such Reward Found");
                    }
                    else
                    {
                        daysRewardCustomData = JsonUtility.FromJson<RewardCustomData>(result.FunctionResult.ToString());
                        TestRewardData(result.FunctionResult.ToString());
                        for (int i = 0; i < daysRewardCustomData.items.Count; i++)
                        {
                            if (daysRewardCustomData.items[i].itemName == "Coins")
                            {
                                //GameManager.TotalCoin += int.Parse(daysRewardCustomData.items[i].itemId);
                                coins += int.Parse(daysRewardCustomData.items[i].itemId);
                            }
                            else if (daysRewardCustomData.items[i].itemName == "Darts")
                            {
                                //GameManager.TotalCoin += int.Parse(daysRewardCustomData.items[i].itemId);
                                darts += daysRewardCustomData.items[i].quantity;
                            }
                            else if (daysRewardCustomData.items[i].itemName == "Hints")
                            {
                                //GameManager.TotalCoin += int.Parse(daysRewardCustomData.items[i].itemId);
                                hints += daysRewardCustomData.items[i].quantity;
                            }
                            else if (daysRewardCustomData.items[i].itemName == "Timers")
                            {
                                ///GameManager.TotalCoin += int.Parse(daysRewardCustomData.items[i].itemId);
                                timers += daysRewardCustomData.items[i].quantity;
                            }
                            else if (daysRewardCustomData.items[i].itemName == "Lives")
                            {
                                //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                life += daysRewardCustomData.items[i].quantity;
                            }
                            else if (daysRewardCustomData.items[i].itemName == "Life")
                            {
                                if (daysRewardCustomData.items[i].itemId == "L1/2")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDays = "L1/2";
                                }
                                else if (daysRewardCustomData.items[i].itemId == "L0")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDays = "L0";
                                }
                                else if (daysRewardCustomData.items[i].itemId == "L1")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDays = "L1";
                                }
                                else if (daysRewardCustomData.items[i].itemId == "L2")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDays = "L2";
                                }
                                else if (daysRewardCustomData.items[i].itemId == "L3")
                                {
                                    //GameManager.TotalLife += int.Parse(daysRewardCustomData.items[i].itemId);
                                    infiniteLifeStrDays = "L3";
                                }
                            }
                        }
                        //DailyRewardPopUp.SetActive(true);
                    }
                }
                break;
            case "ClaimDailyReward":
                {
                }
                break;
            case "GetRewardsCustomData":
                {
                    Debug.Log("GetRewardsCustomData : " + result.FunctionResult.ToString());
                    CustomDatas = JsonUtility.FromJson<CustomDataServer>(result.FunctionResult.ToString());

                    for (int i = 0; i < CustomDatas.DailyReward.Count; i++)
                    {
                        for (int j = 0; j < CustomDatas.DailyReward[i].items.Count; j++)
                        {
                            if (CustomDatas.DailyReward[i].items[j].itemId == "CO")
                            {
                                if (i < dailyRewardItems.Count)
                                {
                                    dailyRewardItems[i].GetComponent<DailyRewardItem>().coinText.text = "x" + CustomDatas.DailyReward[i].items[j].quantity.ToString();
                                }
                            }
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Callback on failed response from the server
    /// </summary>
    /// <param name="error"></param>
    public void OnFailedResponse(PlayFabError error)
    {
        Debug.Log(error.Error);
    }
/// <summary>
/// Get custom data of the items 
/// </summary>
    public void GetRewardsCustomData()
    {
        ExecuteCloudScriptRequest GetRewardsCustomDataContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetRewardsCustomData"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetRewardsCustomDataContainer, OnSuccessfulResponse, OnFailedResponse);
    }
/// <summary>
/// Set player reward data on the Playfab server 
/// </summary>
    public void SetDailyRewardDataOnServer()
    {
        ExecuteCloudScriptRequest SetDailyRewardDataOnServerContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "SetDailyRewardData",
            FunctionParameter = new
            {
                Data = JsonUtility.ToJson(rewardData)
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(SetDailyRewardDataOnServerContainer, OnSuccessfulResponse, OnFailedResponse);
    }
/// <summary>
/// Get DailyRewardData of the current player
/// </summary>
    public void GetDailyRewardData()
    {
        ExecuteCloudScriptRequest GetDailyRewardContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetDailyReward"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetDailyRewardContainer, OnSuccessfulResponse, OnFailedResponse);
    }
/// <summary>
/// Get DaysRewardData of the current player
/// </summary>
    public void GetDaysRewardData()
    {
        ExecuteCloudScriptRequest GetDaysRewardContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetDaysReward"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetDaysRewardContainer, OnSuccessfulResponse, OnFailedResponse);
    }
/// <summary>
/// Get DailyRewardData from the server
/// </summary>
    public void GetDailyRewardDataServer()
    {
        ExecuteCloudScriptRequest GetDailyRewardDataServerContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetDailyRewardDataFromServer"
        };
        PlayFabClientAPI.ExecuteCloudScript(GetDailyRewardDataServerContainer, OnSuccessfulResponse, OnFailedResponse);
    }
/// <summary>
/// Debug Results
/// </summary>
/// <param name="DebugReward">JSON here</param>
    public void TestRewardData(string DebugReward)
    {
        Debug.LogError("Test Daily reward: " + DebugReward);
        //Debugger.Instance.Debug(DebugReward);
    }

    void FalseInfoTutorialScreen()
    {
        infoTurotialScreen.SetActive(false);
    }

    public void GetCurrentReward()
    {
        GiveInfiniteLifeReward();

        //GameManager.TotalCoin += coins;
        // if (coins > 0)
        // {
        //     VFXManager.Instance.PlayCoinParticle(coins);
        //     // cion nu particle ahiya play krva nu che
        // }
        // GameManager.TotalLife += life;
        // GameManager.TotalDirt += darts;
        // GameManager.TotalHint += hints;
        // GameManager.TotalTimer += timers;
        // GameManager.Instance.SaveUserData();
        // DailyRewardPopUp.SetActive(false);

        // if (BoardingSystem.Instance._boardingData.isStarbox == false && PlayFabManager.Instance.currentMember.MemberPersonalData.Level == 4)
        // {
        //     BoardingSystem.Instance.InfoPanel(BoardingSystem.Instance.Satrbox, true, 0.5f);
        //     BoardingSystem.Instance.EncodeData();
        // }
    }

    public void GiveInfiniteLifeReward()
    {
        switch (infiniteLifeStrDaily)
        {
            case "L1/2":
                Debug.LogError("Consumed life called here 5");
            //    InfiniteLivesController.Instance.ConsumeBHalf();
                break;
            case "L0":
                Debug.LogError("Consumed life called here 6");
              //  InfiniteLivesController.Instance.ConsumeB0();
                break;
            case "L1":
                Debug.LogError("Consumed life called here 7");
                //InfiniteLivesController.Instance.ConumeInfiniteLife(InfiniteLife.InfiniteLifeType.B1);
                break;
            case "L2":
                Debug.LogError("Consumed life called here 8");
                //InfiniteLivesController.Instance.ConumeInfiniteLife(InfiniteLife.InfiniteLifeType.B2);
                break;
            case "L3":
                Debug.LogError("Consumed life called here 9");
              //  InfiniteLivesController.Instance.ConumeInfiniteLife(InfiniteLife.InfiniteLifeType.B3);
                break;
            default:
                break;
        }

        switch (infiniteLifeStrDays)
        {
            case "L1/2":
                Debug.LogError("Consumed life called here 18");
            //    InfiniteLivesController.Instance.ConsumeBHalf();
                break;
            case "L0":
                Debug.LogError("Consumed life called here 19");
              //  InfiniteLivesController.Instance.ConsumeB0();
                break;
            case "L1":
                Debug.LogError("Consumed life called here 20");
            //    InfiniteLivesController.Instance.ConumeInfiniteLife(InfiniteLife.InfiniteLifeType.B1);
                break;
            case "L2":
                Debug.LogError("Consumed life called here 21");
               // InfiniteLivesController.Instance.ConumeInfiniteLife(InfiniteLife.InfiniteLifeType.B2);
                break;
            case "L3":
                Debug.LogError("Consumed life called here 22");
             //   InfiniteLivesController.Instance.ConumeInfiniteLife(InfiniteLife.InfiniteLifeType.B3);
                break;
            default:
                break;
        }
    }

    public void Get2XReward()
    {
        DailyRewardPopUp.SetActive(false);
        // IronSourceAdManager.Instance.ShowRewardVideo("2XDailyReward");
        // //show reward video here
        // //coins,hints,darts,timers,life x 2 as reward
        // if (BoardingSystem.Instance._boardingData.isStarbox == false && PlayFabManager.Instance.currentMember.MemberPersonalData.Level == 4)
        // {
        //     BoardingSystem.Instance.InfoPanel(BoardingSystem.Instance.Satrbox, true, 0.5f);
        //     BoardingSystem.Instance.EncodeData();
        // }
    }

    void ResetRewards()
    {
        coins = 0;
        life = 0;
        darts = 0;
        timers = 0;
        hints = 0;
    }

    void SetUI()
    {
        for (int i = 0; i < dailyRewardItems.Count; i++)
        {
            if (i == (rewardData.ConsecutiveOnlineDays))
            {
                dailyRewardItems[i].SetActive(true, false);
            }
            else if (i < rewardData.ConsecutiveOnlineDays)
            {
                dailyRewardItems[i].SetActive(false, true);
            }
            else
            {
                dailyRewardItems[i].SetActive(false, false);
            }
        }
        //float fillAmount = (float)(rewardData.ConsecutiveOnlineDays + 1) / (float)7;
        //Debug.LogError("DailyRewardFillAmount:" + fillAmount + ":mul:" + (fillAmount * 615) + ":onlineDays:"+rewardData.ConsecutiveOnlineDays);
        //sliderImage.GetComponent<RectTransform>().sizeDelta = new Vector2(fillAmount * 615, 25);
        //dayCountText.text = (rewardData.ConsecutiveOnlineDays + 1) + "";

        //Debug.LogError("Daily Reward setui isreturnin:" + rewardData.IsReturningUser);
        if (rewardData.IsReturningUser)
        {
            titleText.text = "Welcome Back!";
        }
        else
        {
            titleText.text = "Daily Reward!";
        }


        int maxDays = 0;
        int minDays = 0;
        int currentValue = 0;
        if (rewardData.TotalOnlineDays <= daysRewardInt[0])
        {
            //7
            currentValue = rewardData.TotalOnlineDays;
            maxDays = daysRewardInt[0];
        }
        else if (rewardData.TotalOnlineDays <= daysRewardInt[1])
        {
            //15
            currentValue = rewardData.TotalOnlineDays;
            maxDays = daysRewardInt[1];
            minDays = daysRewardInt[0];
        }
        else if (rewardData.TotalOnlineDays <= daysRewardInt[2])
        {
            //22
            currentValue = rewardData.TotalOnlineDays;
            maxDays = daysRewardInt[2];
            minDays = daysRewardInt[1];
        }
        else if (rewardData.TotalOnlineDays <= daysRewardInt[3])
        {
            //30
            currentValue = rewardData.TotalOnlineDays;
            maxDays = daysRewardInt[3];
            minDays = daysRewardInt[2];
        }
        else
        {
            currentValue = rewardData.TotalOnlineDays;
            //30/10 3+1*10
            if (rewardData.TotalOnlineDays % 10 == 0)
            {
                maxDays = (rewardData.TotalOnlineDays / 10) * 10;
            }
            else
            {
                maxDays = ((rewardData.TotalOnlineDays / 10) + 1) * 10;
            }
            minDays = maxDays - 10;
        }

        float fillAmountMain = (float)(currentValue - minDays) / (float)(maxDays - minDays);
        Debug.LogError(fillAmountMain + "  min : " + minDays + "     Max : " + maxDays + "    current :  " + currentValue);
        sliderImage.GetComponent<RectTransform>().sizeDelta = new Vector2(fillAmountMain * 615, 25);
        dayCountText.text = rewardData.TotalOnlineDays.ToString();
        maxDaysText.text = "" + maxDays;
    }
}

/// <summary>
/// Class To Store Daily Reward Data
/// </summary>
[System.Serializable]
public class RewardData
{
    public int ConsecutiveOnlineDays;
    public int TotalOnlineDays;
    public string LastLogin;
    public bool IsDailyRewardClaimed;
    public bool IsDaysRewardClaimed;
    public bool IsReturningUser;
    public int SecondsLeftInNextDay;
    public RewardData()
    {
        ConsecutiveOnlineDays = 0;
        TotalOnlineDays = 0;
        LastLogin = "";
        IsDailyRewardClaimed = false;
        IsDaysRewardClaimed = false;
        IsReturningUser = false;
        SecondsLeftInNextDay = 0;
    }
}