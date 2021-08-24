using System.Collections;
using System.Collections.Generic;
using System;
     
[Serializable]
/// <summary>
/// Card containing data of card
/// </summary>
public class AllCard
{
    public int starRequired;
    public int starAcquired;
    public string cardName;
}
[Serializable]
/// <summary>
/// Class containing data of city
/// </summary>
public class CityData
{
    public string cityName;
    public int outfitStarRequired;
    public int outfitStarTimeLimit;
    public string outfitStarMaxTime;
    public int outfitStarAcquired;
    public int badgeStarRequired;
    public int badgeStarAcquired;
    public int frameIndex;
    public List<AllCard> allCards=new List<AllCard>();
    public int outfitStarRemainingTime;
}

[Serializable]
/// <summary>
/// Class containing all cities list
/// </summary>
public class CitiesContainerClass
{
    public List<CityData> allCities = new List<CityData>();
}

[Serializable]
/// <summary>
/// Class contiaining list of names of cities 
/// </summary>
public class CitiesNames
{
    public List<string> allCitiesNames = new List<string>();
}
