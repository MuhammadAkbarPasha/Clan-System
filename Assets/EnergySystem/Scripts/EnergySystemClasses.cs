using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    // Start is called before the first frame update
    [System.Serializable]
    public class LevelSetup
    {
        public int Level;
        public int Consumption;
        public int Capacity;

    }
[System.Serializable]
    public class LevelsData
    {
        public List<LevelSetup> PlayerLevelsSetup;
    }
