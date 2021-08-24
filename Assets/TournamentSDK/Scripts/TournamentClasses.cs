using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Tournament
{
    public List<SinglePlayerData> singlePlayerData;
}

[System.Serializable]
public class SinglePlayerData
{
    public string Id;
    public string LeaderBoardName;
    public string DisplayName;
    public int Value;
    public bool IsthisCurrentPlayer;
    public int Position;
    public Profile profile;
}


[System.Serializable]
public class PlayerStat
{
    public string PlayerTitleId;
    public string GroupUsername;
    public int Stars;
    public bool Claimed;
    public bool isCurrentPlayer;
}

[System.Serializable]
public class TeamEvent
{
    public int TotalStars;
    public int TargetStars;
    public bool GoalAchieved;
    public string LastUpdated;
    public List<PlayerStat> PlayerStats = new List<PlayerStat>();
}