using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BGGamesCore.Matchmaker;
using System;
using System.Runtime.CompilerServices;

public class UIPlayerInfoCard : MonoBehaviour
{
    [SerializeField] private Text playerNameText = null;
    [SerializeField] private Text playerMMRText = null;
    [SerializeField] private Text playerCategoryText = null;
    [SerializeField] private Text playerPartyAttack = null;
    [SerializeField] private Image playerAvatarImage = null;

    [SerializeField]    List<GameObject> hasEntryDataObject     = new List<GameObject>();
    [SerializeField]    List<GameObject> NoEntryDataObject      = new List<GameObject>();

    PlayerMatchmakerData playerData;

    /// <summary>
    /// Used by the revenge system to remove an element from its popup
    /// </summary>
    private System.Action<string> onTapped = null;
    public string GetPlayerId
    {
        get
        {
            return playerData.playFabId;
        }
    }

    private void OnDestroy()
    {
        onTapped = null;
    }

    public void Initialize(PlayerMatchmakerData playerData,bool isPlayer1=false)
    {
        this.playerData = playerData;
        Debug.LogError("MMR Is"+ playerData.mmr);
        Initialize(playerData.name, "abc", playerData.mmr.ToString(), "xcd",playerData.avatarUrl,isPlayer1);

    }

    public void Initialize(string playerName, string partyAttack,string playerMMR, string playerCategory, string playerAvatarKey, bool isPlayer1)
    {
        this.playerNameText.text = playerName;
        this.playerMMRText.text = "MMR:" + playerMMR;
        if (!isPlayer1)
            StartCoroutine(GetAvatar(playerAvatarKey)); // For Player2
        else
            SetPlayer1Avatar();

        ToggleView(true);



    }




    /// <summary>
    /// This is where you can add your logic of getting avatar
    /// </summary>
    public IEnumerator GetAvatar(string avatarKey) 
    {
        if (Uri.IsWellFormedUriString(avatarKey, UriKind.Absolute))
        {
            WWW www = new WWW(avatarKey);
            yield return www;
            Sprite sprite= Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
            playerAvatarImage.sprite = sprite;
            UIBattleLobby.Instance.SetEnemySprite(sprite);

        }
        else 
        {
            //            if it is not avatarKey url, add logic for android devices here
        
        
        } 
    }

    public void SetPlayer1Avatar() 
    {
        playerAvatarImage.sprite = UIBattleLobby.Instance.GetPlayerSprite();


    }
    public void Initialize(string playerName) 
    {
        this.playerNameText.text = playerName;
    
    
    }
    public void AddListener(System.Action<string> onTapped)
    {
        this.onTapped += onTapped;
    }

    public void ToggleView(bool hasData)
    {
        
        foreach (GameObject gb in hasEntryDataObject) 
        {
            gb.SetActive(hasData);
        
        
        }
        foreach (GameObject gb in NoEntryDataObject)
        {
            gb.SetActive(!hasData);


        }
    }

    /// <summary>
    /// For sample revenge system scene
    /// </summary>
    public void OnFightClicked()
    {
        onTapped(playerData.playFabId);

        GameDataManager.Instance.isDoingRevenge = true;
        GameDataManager.Instance.revengeId = playerData.playFabId;
        Debug.Log("PlayfabID:"+playerData.playFabId+"Player:" + playerData.name + " is your enemy in the next game!");
    }
}
