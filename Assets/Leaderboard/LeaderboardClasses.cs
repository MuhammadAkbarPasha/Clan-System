using System.Collections.Generic;
using System.Collections;
using UnityEngine;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
[System.Serializable]

/// <summary>
/// Player profile class
/// </summary>
public class Profile
{
    public string PublisherId ;
    public string TitleId ;
    public string PlayerId ;
    public string DisplayName ;
    public string AvatarUrl ;
}
[System.Serializable]
/// <summary>
/// Player leaderboard data class
/// </summary>
public class Leaderboard
{
    public string PlayFabId ;
    public string DisplayName ;
    public int StatValue ;
    public int Position ;
    public Profile Profile ;
}
[System.Serializable]
/// <summary>
/// Leaderboard list holder class 
/// </summary>
public class LeaderboardData
{
    public List<Leaderboard> Leaderboard;

}

