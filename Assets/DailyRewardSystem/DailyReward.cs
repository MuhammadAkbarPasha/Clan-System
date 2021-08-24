using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
/// <summary>
/// Required information about the item class
/// </summary>
public class RewardCustomData
{
    public string offerTitle;
    public string bgColor;
    public string midBgImage;
    public string discountImage;
    public List<RewardItem> items = new List<RewardItem>();
    public string originalPrice;
    public string offerPrice;
    public int timer; // this is only needed in the case of seasonal offer.
}
[System.Serializable]
/// <summary>
/// Class holding data of the daily and days reward from the server
/// </summary>
public class CustomDataServer
{
    public List<RewardCustomData> DailyReward;
    public List<RewardCustomData> DaysPlayedReward;
}
[System.Serializable]
/// <summary>
/// Single reward item 
/// </summary>
public class RewardItem
{
    public string itemId;
    public string itemName;
    public int quantity;
}
[System.Serializable]
/// <summary>
/// Reward Identity
/// </summary>
public class DailyReward
{
    public int Day;
    public string RewardId;
}
