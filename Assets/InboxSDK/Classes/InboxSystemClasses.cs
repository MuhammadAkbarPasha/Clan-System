using System.Collections.Generic;
using StoreSystem;

[System.Serializable]
/// <summary>
/// Player gift from server data class
/// </summary>
public class Gift
{
    public string Name;
    public string InstanceId;
    public string Message;
    public string RemainingTime;
    public string Description;
    public List<Item> items = new List<Item>();
}
[System.Serializable]
/// <summary>
/// Response from the server class
/// </summary>
public class GiftsResponse
{
    public List<Gift> Gifts = new List<Gift>();
}