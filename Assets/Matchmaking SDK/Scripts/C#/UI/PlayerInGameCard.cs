using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameCard : MonoBehaviour
{

    [SerializeField]
    private Text PlayerName;

    public void SetData(string name)
    {
        PlayerName.text = name;
    
    }

}
