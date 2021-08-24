using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EmoteScript : MonoBehaviour
{
    public int id=0;


    /// <summary>
    /// 
    /// 1- Pass the ID to EmotesManager to show emote from local storage.
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void SetUpButton(int id)
    {
        this.id=id;
        GetComponent<Button>().onClick.AddListener(()=>{
            EmotesManager.Instance.ShowEmoteLocal(id);
        });
    }
}
