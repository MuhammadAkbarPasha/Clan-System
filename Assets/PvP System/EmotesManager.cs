using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EmotesManager : MonoBehaviour
{

    public static EmotesManager Instance;

    public GameObject YouEmotesParent;
    public GameObject EmoteObject;
    public List<Sprite> emotes = new List<Sprite>();

    public GameObject LocalEmote;
    public GameObject RemoteEmote;

    public GameObject LocalMessage;
    public GameObject RemoteMessage;

    private IEnumerator coroutine;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void ShowEmoteLocal(int id)
    {
        Debug.Log("Emote Id: " + id);
        LocalEmote.transform.GetChild(0).GetComponent<Image>().sprite = emotes[id];
        LocalEmote.GetComponent<Animator>().SetTrigger("play");
        LocalMessage.SetActive(false);
        LocalEmote.SetActive(true);
        PlayersScoreController.Instance.CallEmoteRPC(id);
    }

    public void ShowEmoteRemote(int id)
    {

        RemoteEmote.transform.GetChild(0).GetComponent<Image>().sprite = emotes[id];
        RemoteEmote.GetComponent<Animator>().SetTrigger("play");
        RemoteMessage.SetActive(false);
        RemoteEmote.SetActive(true);
    }
    public void ShowEmoteLocal(GameObject gameObject)
    {
        string message="";
        message=gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        if(message.Length==0)
        return;
        LocalEmote.SetActive(false);
        LocalMessage.GetComponentInChildren<TextMeshProUGUI>().text = message;
        LocalMessage.SetActive(true);
        LocalMessage.GetComponent<Animator>().SetTrigger("play");
       
        PlayersScoreController.Instance.CallMessageRPC(message);
    }

    public void ShowEmoteRemote(string message)
    {
        RemoteEmote.SetActive(false);
        RemoteMessage.SetActive(true);
        RemoteMessage.GetComponentInChildren<TextMeshProUGUI>().text = message;
        RemoteMessage.GetComponent<Animator>().SetTrigger("play");
        RemoteMessage.SetActive(true);
    }

    public void ShowEmotesToggle()
    {

        YouEmotesParent.SetActive(YouEmotesParent.activeInHierarchy == true ? false : true);


    }

    public void SetEmotes()
    {
        if (YouEmotesParent == null)
        {
            Debug.LogError("Local Emotes Parent Missing");
            return;
        }

        for (int i = 0; i < emotes.Count; i++)
        {
            GameObject emoteObject = Instantiate(EmoteObject, YouEmotesParent.transform);
            emoteObject.AddComponent<EmoteScript>();
            emoteObject.GetComponent<Image>().sprite = emotes[i];
            emoteObject.GetComponent<EmoteScript>().SetUpButton(i);
        }


        YouEmotesParent.SetActive(false);

    }
}