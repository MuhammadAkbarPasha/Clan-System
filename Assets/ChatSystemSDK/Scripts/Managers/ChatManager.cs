using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using ExitGames.Client.Photon.Encryption;
using Photon.Chat.UtilityScripts;
using System;
using System.Globalization;

[Serializable]
public class FriendStatus
{
    public string FriendName;
    public string Status = "Offline";

    public FriendStatus(string friendName, string status)
    {
        this.FriendName = friendName;
        this.Status = status;
    }

    public FriendStatus(string friendName)
    {
        this.FriendName = friendName;
    }
}

public class GroupUpdateType
{
    public enum UpdateType { Joined = 0, Left = 1, Kicked = 2 }
}
[System.Serializable]
/// <summary>
/// Message Class containg all the necessary about the message 
/// </summary>
public class MessageItem
{
    public string senderName;
    public string senderPlayfabId;
    public string message;
    public string messageTimestampstr;
    public bool islifeRequest = false; //false : Message, true : LifeRequest 
    /// <summary>
    /// Encode Object of this class to JSON
    /// </summary>
    /// <returns></returns>
    public string MakeJson()
    {
        return JsonUtility.ToJson(this);
    }
    /// <summary>
    /// Decode JSON to object of this class
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public MessageItem returnObject(string json)
    {
        return JsonUtility.FromJson<MessageItem>(json);
    }

    public MessageItem(bool islifeRequest, string senderName = "", string message = "", string senderPlayfabId = "", string timeStamp = "")
    {
        this.islifeRequest = islifeRequest;
        this.senderName = senderName;
        this.message = message;
        this.senderPlayfabId = senderPlayfabId;
        this.messageTimestampstr = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US"));
    }

    public MessageItem()
    {
    }
}
[System.Serializable]
public class MessageList
{
    public List<MessageItem> Messages;
}


public sealed class ChatManager : MonoBehaviour, IChatClientListener
{
    #region Private_VARIABLES
    /// <summary>
    /// Group the player is in
    /// </summary>
    private string GroupName;
    /// <summary>
    /// ChatClient object of this class.
    /// </summary>
    private ChatClient chatClient;
    #endregion
    #region PUBLIC_VARIABLES
    /// <summary>
    /// UI InputField. Used to get message from the text component 
    /// </summary>
    public InputField messageToSend;
    /// <summary>
    /// MessageList object of this class
    /// </summary>
    public MessageList DownloadedMessages;
    /// <summary>
    /// Static reference to this class
    /// </summary>
    public static ChatManager Instance;
    /// <summary>
    /// Helping field to control the flow of logic
    /// </summary>
    public bool NewMember = false;

    #endregion

    #region Public_Methods

    public void setgroupName(string groupName)
    {
        this.GroupName = groupName;
    }

    public string getgroupName()
    {
        return GroupName;
    }


    /// <summary>
    /// Unsubscribe from a channel
    /// </summary>
    /// <param name="groupName"></param>
    public void Unsubscribe()
    {
        if (chatClient == null)
            return;
        chatClient.Unsubscribe(new string[] { GroupName });
        DownloadedMessages = new MessageList();
    }

    public void Awake()
    {
        Instance = this;
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //Debug.LogError(level.ToString() + "/-/" + message);
    }

    
    /// <summary>
    /// State change callback
    /// </summary>
    /// <param name="state"></param>
    public void OnChatStateChange(ChatState state)
    {
        //Debug.LogError(state.ToString());
    }

    [Header("Friends")]
    [SerializeField]
    List<FriendStatus> friends = new List<FriendStatus>();

    [Header("Count Of Online Friends")]
    public int TotalOnlineFriends = 0;
    /// <summary>
    /// Onconndeected callback
    /// </summary>
    public void OnConnected()
    {
        //Debug.LogError("on cunnected in0:" + PlayFabManager.Instance.currentMember.MemberGroup + ":member id:" + PlayFabManager.Instance.currentMember.MemberGroup.Id + ":GroupName:" + GroupName);
        if (PlayFabManager.Instance.currentMember.MemberGroup == null)
            return;

        AddFriends();

        setgroupName(PlayFabManager.Instance.currentMember.MemberGroup.Id);
        if (!string.IsNullOrEmpty(GroupName))
        {
            SubscribeToCurrentGroup(GroupName);
        }
        //Debug.LogError("on cunnected in2:" + PlayFabManager.Instance.currentMember.MemberGroup);
        InvokeRepeating(nameof(InvokeFunc), 10, 10);
    }
    /// <summary>
    /// Add all the users as friends in the group
    /// </summary>
    public void AddFriends()
    {
        List<string> friendsList = new List<string>();
        foreach (MemberData md in ClanSystemController.Instance.playFabClanData.groupMembers.groupMembers)
        {
            friends.Add(new FriendStatus(md.GroupUsername));
            friendsList.Add(md.GroupUsername);
        }
        chatClient.AddFriends(friendsList.ToArray());
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }
    /// <summary>
    /// Add one of the available users as friend
    /// </summary>
    /// <param name="friendUsername">friend username</param>
    public void AddFriends(string friendUsername)
    {
        friends.Add(new FriendStatus(friendUsername, "Online"));
        List<string> friendsList = new List<string>();
        friendsList.Add(friendUsername);
        chatClient.AddFriends(friendsList.ToArray());
    }


    /// <summary>
    /// Try receiving  Channels
    /// </summary>
    public void InvokeFunc()
    {
        ChatChannel chatChannel;
        if (!string.IsNullOrEmpty(GroupName))
        {
            chatClient.TryGetChannel(GroupName, out chatChannel);
        }
        //Debug.LogError( "Subscribers Are"+   JsonUtility.ToJson(aas.Subscribers));
    }

    /// <summary>
    /// subscribtion to a channel is made here
    /// </summary>
    /// <param name="groupName"></param>
    private void SubscribeToCurrentGroup(string groupName)
    {
        Debug.Log("SUBSCRIBED");
        if (PlayFabManager.Instance.currentMember.getChatEnabled())
        {
            ClanSystemController.Instance.DownloadMessagesFromSharedGroupObject(groupName);
        }
        chatClient.Subscribe(new string[] { groupName });

        Debug.Log("ever come here For Chat");
        chatClient.SetOnlineStatus(2);

        //Debug.LogError("after joing  subscribe to load request life2222......:");
        //PlayPabUiController.Instance.GetLifeRequests();        
    }


    /// <summary>
    /// Ondisconnected callback
    /// </summary>
    public void OnDisconnected()
    {
        chatClient.SetOnlineStatus(0);
        //throw new System.NotImplementedException();
        //Debug.LogError(chatClient.DisconnectedCause);
    }


    /// <summary>
    /// On Get Messages callback , this is called on both receiving and sending ends
    /// </summary>
    /// <param name="channelName"></param>
    /// <param name="senders"></param>
    /// <param name="messages"></param>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string messags = "";
        for (int i = 0; i < senders.Length; i++)
        {
            messags = string.Format("{0}", messages[i]);
        }

        string temp = messags;
        MessageItem message = new MessageItem();
        message = JsonUtility.FromJson<MessageItem>(temp);

        if (!message.islifeRequest)
        {
            if (PlayFabManager.Instance.currentMember.getChatEnabled())
                PopulateMsg(message);
            return;
        }

        //need to cater here for life message
    }


    /// <summary>
    /// Onsend private message callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <param name="channelName"></param>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Status change callback
    /// </summary>
    /// <param name="user">User whose status changed</param>
    /// <param name="status">changed status</param>
    /// <param name="gotMessage"></param>
    /// <param name="message"></param>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
        if (friends.Exists(obj => obj.FriendName == user))
        {
            if (status == 0)
            {
                friends.Find(obj => obj.FriendName == user).Status = "Offline";
            }
            if (status == 2)
            {
                friends.Find(obj => obj.FriendName == user).Status = "Online";
                setTotalFriendsOnline(TotalOnlineFriends = friends.FindAll(obj => obj.Status == "Online").Count);
            }
        }
    }


    /// <summary>
    /// Sets the total friends online
    /// </summary>
    /// <param name="n">count friends</param>
    public void setTotalFriendsOnline(int n)
    {
        this.TotalOnlineFriends = n;
    }


    /// <summary>
    /// To fetch the count of friends online
    /// </summary>
    /// <returns>count of friends </returns>
    public int getTotalFriendsOnline()
    {
        return this.TotalOnlineFriends;
    }

    public void SetOnline()
    {
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void SetOffline()
    {
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }


    /// <summary>
    /// Onsubscribed callback
    /// </summary>
    /// <param name="channels"></param>
    /// <param name="results"></param>
    public void OnSubscribed(string[] channels, bool[] results)
    {
        if (NewMember)
        {
            GroupUpdate(PlayFabManager.Instance.currentMember.GetGroupUsername(), GroupUpdateType.UpdateType.Joined);
            NewMember = false;
        }

        if (string.IsNullOrEmpty(GroupName))
        {
            return;
        }

        ChatChannel aas;
        chatClient.TryGetChannel(GroupName, out aas);
        aas.PublishSubscribers = true;
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }


    /// <summary>
    /// OnUnsubscribed callbacl
    /// </summary>
    /// <param name="channels"></param>
    public void OnUnsubscribed(string[] channels)
    {

    }


    /// <summary>
    /// On User subscribed callback
    /// </summary>
    /// <param name="channel">Channel user subscribed to</param>
    /// <param name="user">User which subscribed</param>
    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// On Userunsubscribed callback
    /// </summary>
    /// <param name="channel">Channel player unsubscribed from</param>
    /// <param name="user">User which unsubscribed</param>
    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Send message function message item class object is converted to json here
    /// </summary>
    public void SendMessages()
    {
        MessageItem newMessage = new MessageItem(false, PlayFabManager.Instance.currentMember.GetGroupUsername(), messageToSend.text);
        messageToSend.text = "";
        chatClient.PublishMessage(GroupName, newMessage.MakeJson());
        Debug.Log(GroupName + "Group Name is");
        ClanSystemController.Instance.UploadMessageToSharedGroupObject(GroupName, newMessage.MakeJson());

    }


    /// <summary>
    /// Send message function
    /// </summary>
    /// <param name="message">message to send in string format</param>
    public void SendMessagesByPassingValue(string message)
    {
        Debug.LogError("send msg stinng:" + message + ":current id:" + PlayFabManager.Instance.currentMember.Id + ":group name:" + GroupName);
        MessageItem newMessage = new MessageItem(false, PlayFabManager.Instance.currentMember.GetGroupUsername(), message, PlayFabManager.Instance.currentMember.Id);
        chatClient.PublishMessage(GroupName, newMessage.MakeJson());
        Debug.Log(GroupName + "Group Name is");
        ClanSystemController.Instance.UploadMessageToSharedGroupObject(GroupName, newMessage.MakeJson());
    }


    /// <summary>
    /// Send Group updates as messages. Such as Joining leaving or kicking
    /// </summary>
    /// <param name="groupUsername"></param>
    /// <param name="updateType"></param>
    public void GroupUpdate(string groupUsername, GroupUpdateType.UpdateType updateType)
    {
        MessageItem newMessage;

        switch (updateType)
        {
            case GroupUpdateType.UpdateType.Joined:
                {
                    newMessage = new MessageItem(false, "adminSend", groupUsername + " Joined the Team");
                }
                break;
            case GroupUpdateType.UpdateType.Left:
                {
                    newMessage = new MessageItem(false, "adminSend", groupUsername + " Left the Team");
                    chatClient.PublishMessage(GroupName, newMessage.MakeJson());
                    ClanSystemController.Instance.UploadMessageToSharedGroupObject(GroupName, newMessage.MakeJson());
                    Unsubscribe();
                    return;
                }

            case GroupUpdateType.UpdateType.Kicked:
                {
                    newMessage = new MessageItem(false, "adminSend", groupUsername + " was kicked from the team");
                }
                break;
            default:
                {
                    return;
                }
        }

        chatClient.PublishMessage(GroupName, newMessage.MakeJson());
        ClanSystemController.Instance.UploadMessageToSharedGroupObject(GroupName, newMessage.MakeJson());
    }


    /// <summary>
    /// Make request for Energy
    /// </summary>
    public void sendEnergyRequest()
    {
        MessageItem newMessage = new MessageItem(true);
        chatClient.PublishMessage(GroupName, newMessage.MakeJson());
    }


    /// <summary>
    /// Authenticate this user with the playfab
    /// </summary>
    /// <param name="token"></param>
    public void AuthenticateWithPlayfab(string token)
    {
        chatClient = new ChatClient(this);
        chatClient.ChatRegion = "EU";
        var auth = new AuthenticationValues();
        auth.AuthType = CustomAuthenticationType.Custom;
        auth.AddAuthParameter("username", PlayFabManager.Instance.PlayFabUserId);
        auth.AddAuthParameter("token", token);
        Debug.Log("Username Is " + PlayFabManager.Instance.currentMember.GetGroupUsername());
        auth.UserId = PlayFabManager.Instance.currentMember.GetGroupUsername();
        GroupName = PlayFabManager.Instance.currentMember.MemberGroup.Id;
        chatClient.Connect(PlayFabManager.Instance.PhotonAppId, "1.0", new Photon.Chat.AuthenticationValues(auth.UserId));
    }


    /// <summary>
    /// Populate in the ui
    /// </summary>
    /// <param name="msg"></param>
    private void PopulateMsg(MessageItem msg)
    {
        //UiController.Instance.showMessage(msg);
    }


    /// <summary>
    /// Update is used for keeping the chat client service working
    /// </summary>
    public void Update()
    {
        if (chatClient != null)
            chatClient.Service();
    }


    /// <summary>
    /// Callback for ChatDisable
    /// </summary>
    public void OnChatDisabled() // clears previouslist
    {
        DownloadedMessages.Messages.Clear();
        //UiController.Instance.removeAllFromSnap(UiController.Instance.verticalScrollSnapMessages); // Optional For you, only here for testing purposes

    }


    /// <summary>
    /// Callback for ChatEnable
    /// </summary>
    public void OnChatEnabled() // downloads the list again 
    {
        ClanSystemController.Instance.DownloadMessagesFromSharedGroupObject(GroupName);
    }

    private void OnApplicationPause(bool pause)
    {
        //Debug.LogError("on application pause:" + pause);
        if (!pause)
        {
            if (chatClient != null)
            {
                chatClient.ConnectToFrontEnd();
            }
        }
    }
    #endregion
}