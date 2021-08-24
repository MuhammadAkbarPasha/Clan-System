using System.Collections;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// All PVP Enums Class
/// </summary>
public class PVPEnums
{



    /// <summary>
    /// Player Identifier
    /// </summary>
    public enum PlayerCurrentState { Player1 = 0, Player2 = 1, None = 2 }
    /// <summary>
    /// Puzzle Identifier
    /// </summary>
    public enum PuzzleTypeEnums { City = 0, New = 1, Mix = 2, General = 3 };
    /// <summary>
    /// Game current status 
    /// </summary>
    public enum GameStatus { Win = 0, Loss = 1, Playing = 2, NotPlaying = 3 };






}
/// <summary>
/// Puzzle data class
/// </summary>
[System.Serializable]
public class PuzzleType
{
    public int percentage;
    public PVPEnums.PuzzleTypeEnums puzzleType;
    /// <summary>
    /// This is only for other devs 
    /// no need to put puzzle type in string add city for city 
    /// and general# for genral types
    /// </summary>
    public string typeInString;

    public PuzzleType(int Percent, PVPEnums.PuzzleTypeEnums PuzzleType, string TypeInString)
    {
        this.percentage = Percent;
        this.puzzleType = PuzzleType;
        this.typeInString = TypeInString;


    }
    public PuzzleType()
    {


    }

}


/// <summary>
/// Player data and reference class 
/// </summary>
[System.Serializable]
public class PlayerInstance  //UI Instance + Photon Player Instance
{
    public RealTimePVP realTimePVP;
    //Used for matching differences of other after they have rejoined 
    public List<int> PlayerFoundDifferences = new List<int>();
}




/// <summary>
/// Data class of the stack system
/// </summary>
[System.Serializable]
public class StackSystem
{
    public List<StackClass> stacks = new List<StackClass>();
    public List<StackClass> GetStack(List<string> generalNames)
    {
        return stacks.Where((st) =>
        generalNames.Any((gn) =>
        gn.Equals(st.name))).ToList();
    }
    public StackClass GetStack(string cityName)
    {
        return stacks.Find((op) => op.name == cityName);
    }

}


/// <summary>
/// Stack data class
/// </summary>
[System.Serializable]
public class StackClass
{


    public StackClass(string name, string type)
    {
        this.name = name;
        this.type = type;

    }
    /// <summary>
    /// Constructor for setting up stack
    /// </summary>
    /// <param name="name">Name of the stack</param>
    /// <param name="type">type shows if it is General or city</param>
    /// <param name="min">Inclusive</param>
    /// <param name="max">Inclusive</param>
    public StackClass(string name, string type, int min, int max)
    {
        this.name = name;
        this.type = type;
        SetUpStack(min, max);
    }

    public StackClass()
    {
    }
    /// <summary>
    /// Both min and max are incluse for creation of levels played
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    private void SetUpStack(int min, int max)
    {
        this.LevelsUnPlayed.Clear();
        for (int i = min; i <= max; i++)
        {
            this.LevelsUnPlayed.Add(i);
        }
    }

    public string name;
    public string type;
    public List<int> LevelsPlayed = new List<int>();
    public List<int> LevelsUnPlayed = new List<int>();
}


/// <summary>
/// General stack type data class
/// </summary>
[System.Serializable]
public class GeneralClass
{
    public string GeneralName;
    public int minRange;
    public int maxRange;

}

[System.Serializable]
/// <summary>
/// General stacks data class. This classs contain meta data for the general stack
/// </summary>
public class AllGenerals
{
    public List<GeneralClass> Generals;
}

/// <summary>
/// Uneeded levels data class
/// </summary>
[System.Serializable]
public class LevelsToBeFilteredClass
{
    public List<int> LevelsToBeFiltered;
}

/// <summary>
/// Level data class
/// </summary>
[System.Serializable]
public class LevelClass
{
    public int levelNumber;
    public string levelType;
    public string StackName;



    public LevelClass(int LevelNumber, string LevelType, string StackName)
    {
        this.levelNumber = LevelNumber;
        this.levelType = LevelType;
        this.StackName = StackName;
    }
}

/// <summary>
/// Foreign data class
/// </summary>
[System.Serializable]
public class GeneralforeignClass
{
    public string GeneralName;
    public int Probability;
}

/// <summary>
/// City data class 
/// </summary>
[System.Serializable]
public class CityClass
{
    public string CityName;
    public int MinRange;
    public int MaxRange;
    public int Probability;
    public List<GeneralforeignClass> GeneralTypesIncluded;



}
/// <summary>
/// All cities data class
/// </summary>
[System.Serializable]
public class RootAllCities
{
    public List<CityClass> AllCities;
}
