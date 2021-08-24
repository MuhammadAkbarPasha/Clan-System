using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;/*
using TeamStatsClasses;*/
using JetBrains.Annotations;
using System;

public class PhotonRoomsManger : MonoBehaviourPunCallbacks
{
    #region DataMembers
    
    private GameObject _roomPrefab;
    private List<RoomList> RoomListButtons = new List<RoomList>();

    public List<RoomInfo> roomsData = new List<RoomInfo>();

 //   private List<RoomList> RoomListButtons = new List<RoomList>();
    private Text _roomNameText;

    private GameObject RoomPrefab
    {
        get { return _roomPrefab; }
    }
    private Text RoomNameText
    {
        get { return _roomNameText; }
    }
    [SerializeField]
    public bool Updated { get; set; }
    // public TeamStats teamStats = new TeamStats();
    #endregion


    [HideInInspector]
    public Text DebugText;
 
    public void Start()
    {
    }
    public void PopulateAvailableRoomsOnJoinedLobby()
    {
    }

    public void CreateRoonm()
    {
    }
    public void OnClickRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            Debug.Log("Player Joined in the Room");
        }
        else
        {
            Debug.Log("Failed to join in the room, please fix the error!");
        }
    }



    /// <summary>
    /// 
    /// 1- Photon call back function to called for any update of room listing while player in a lobby on master clint.
    /// 2- Call the RoomReceived function on each room in a list.
    /// 3- Call RemoveOldRooms function.
    /// 4- Update rooms data.
    /// 
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            RoomReceived(room);
        }
        RemoveOldRooms();
        roomsData = new List<RoomInfo>(roomList);

    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    private void RoomReceived(RoomInfo room)
    {
        Debug.Log("Coming Here On New Room Creation");
        ArenaAndMatchMakingBridge.Instance.DebugText.text += "\nComing Here On New Room Creation Room Name : "+room.Name;
        int index = RoomListButtons.FindIndex(x => x.RoomName == room.Name);
        if (index == -1)
        {
            
            RoomList roomListing = new RoomList();
            //roomListing.SetRoomNameText(room.Name);

            try
            {
                string data = (string)room.CustomProperties["TeamStats"];
                //DateTime
            }
            catch
            {
                Debug.Log("Some Error Here");
            }

            RoomListButtons.Add(roomListing);
            index = (RoomListButtons.Count - 1);
      
        }

        if (index != -1)
        {


            if (room.PlayerCount<1)
            {
                RoomList roomListing = RoomListButtons[index];
                roomListing.Updated = false;

            }
            else {
                RoomList roomListing = RoomListButtons[index];
               // roomListing.SetRoomNameText(room.Name);
                try
                {
                    string data = (string)room.CustomProperties["TeamStats"];
                    //DateTime
                }
                catch
                {
                    Debug.Log("Some Error Here");
                }
                SetupRoomAndTeamJoinButton(room.Name);
            }
        }
    }




    public void SetupRoomAndTeamJoinButton(string roomName) 
    {
        
    }
    public void ShowRoomStats(RoomInfo roomInfo)
    {
        string data = (string)roomInfo.CustomProperties["TeamStats"];
        

    }

    public void DestroyChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);

        }
    }
    public void CompareRooms() 
    {
    
    
    
    
    
    }


    /// <summary>
    /// 
    /// 1- An RPC function to remove old rooms on local and remote instance.
    /// 
    /// </summary>
    [PunRPC]
    public void RemoveOldRooms()
    {
        List<RoomList> removeRooms = new List<RoomList>();

        foreach (RoomList roomListing in RoomListButtons)
        {
            if (!roomListing.Updated)
            {
                removeRooms.Add(roomListing);
            }
            else
            {
                roomListing.Updated = false;
            }
        }

        foreach (RoomList roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListButtons.Remove(roomListing);
            Destroy(roomListingObj);
            Debug.Log(roomListingObj.name + " is Destroyed");
        }


    }

    public string getTimer(int timer)
    {
        float minutes = Mathf.Floor(timer / 60);
        float seconds = (timer % 60);

        return minutes.ToString("00") + ":" + seconds.ToString("00");


    }





}