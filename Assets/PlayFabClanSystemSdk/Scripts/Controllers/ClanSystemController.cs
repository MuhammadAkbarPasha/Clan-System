using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;


public class PlayerRequestData
{

    public int lifeReceived = 0;
    public List<string> playerIds;



    public PlayerRequestData(int lifeReceived, List<string> playerIds)
    {
        this.lifeReceived = lifeReceived;
        this.playerIds = playerIds;

    }
}


public class ClanSystemController : MonoBehaviour
{
    public static ClanSystemController Instance; //Static Instance to the class
    /// <summary>
    /// Reference to current player ClanData 
    /// </summary>
    public PlayFabClanData playFabClanData; // Data Container
    public List<string> MasterPlayerids;
    #region PRIVATE VARIABLES
    //  [SerializeField] PlayPabUiController playPabUicontroller;
    //private UiController uiController;
    /// <summary>
    /// Apply in the group with name or not
    /// </summary>
    [SerializeField] bool applyWithName;
    [SerializeField] int groupUserNameLimit;
    [SerializeField] int levelLimit;
    [Header("GroupInformation")]
    /// <summary>
    /// Promote player to different name or not
    /// </summary>
    [SerializeField] bool promoteMember = false;
    #endregion
    /// <summary>
    /// Selected group information
    /// </summary>
    public NewGroupInformation groupInfo;

    public int GetUserNameLimit()
    {
        return groupUserNameLimit;
    }
    public bool GetApplyWithName()
    {
        return applyWithName;
    }

    /// <summary>
    /// Check if player has already applied in the group or not. Based on the output turn on or off the button
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="button">Button to turn on or not</param>
    public void HasPlayerAppliedAlready(string groupId, GameObject button)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "HasPlayerAppliedAlready",
            FunctionParameter = new
            {
                GroupId = groupId,
            },
            GeneratePlayStreamEvent = true,
        }, result =>
        {
            Debug.Log(result.FunctionResult.ToString());
            switch (result.FunctionResult.ToString())
            {
                case "True":
                    {
                        button.SetActive(false);
                    }
                    break;
                case "False":
                    {
                        button.SetActive(true);
                    }
                    break;
            }

        }, CloudScriptFailure);

    }


    /// <summary>
    /// To Check If This player Has Been Blocked from this group
    /// </summary>
    /// <param name="groupId">Selected Group id         </param>
    /// <param name="button">Button to turn on or not   </param>
    public bool IsPlayerBlockedFromThisGroup(string groupId, GameObject button)
    {

        bool dummy = false;
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "IsPlayerBlockedFromThisGroup",
            FunctionParameter = new
            {
                GroupId = groupId,
            },
            GeneratePlayStreamEvent = true,
        }, result =>
        {
            Debug.Log(result.FunctionResult.ToString());
            switch (result.FunctionResult.ToString())
            {
                case "True":
                    {
                        dummy = true;
                    }
                    break;
                case "False":
                    {
                        dummy = false;

                    }
                    break;

            }
        }, CloudScriptFailure);

        return dummy;
    }


    /// <summary>
    /// Edit group information object of this group
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="imagId">Chosen image id </param>
    /// <param name="description">New description</param>
    /// <param name="requiredLevel">Minimun level the new player need to be on to be a member of this group</param>
    /// <param name="isGroupOpen">Keep this group open or not. If open any player can join directly instaed of waiting for application to be accepted</param>
    public void EditGroupInformation(string groupId, string imagId, string description, int requiredLevel, bool isGroupOpen)
    {
        Debug.LogError("EditGroupInformation groupId:" + groupId + ":imageId:" + imagId + ":des:" + description + ":rl:" + requiredLevel + ":type:" + isGroupOpen);
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "EditGroupInformation",
            FunctionParameter = new
            {
                GroupId = groupId,
                ImageId = imagId,
                Description = description,
                RequiredLevel = requiredLevel,
                isOpen = isGroupOpen,

            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);








    }
    /// <summary>
    /// Change the group name on the Playfab Server
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="newGroupName"></param>
    public void ChangeGroupName(string groupId, string newGroupName)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "ChangeGroupName",
            FunctionParameter = new
            {
                GroupId = groupId,
                NewGroupName = newGroupName
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess
        , CloudScriptFailure); ;
    }






    private void Awake()
    {

        Instance = this;


    }

    /// <summary>
    /// Change the player level. This function also changes level in leaderboards.
    /// </summary>
    /// <param name="newLevel">The new level the player will be on</param>
    public void SetPlayerLevel(int newLevel)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "SetPlayerLevel",
            FunctionParameter = new
            {
                level = newLevel
            },
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Get group information object of the group
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    public void GetGroupInformation(string groupId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "GetGroupInformation",
            FunctionParameter = new
            {
                GroupId = groupId
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Reject Group Application 
    /// </summary>
    /// <param name="groupId">Group Id</param>
    /// <param name="playerId">Title Player Id</param>
    public void RejectGroupApplication(string groupId, string playerId)
    {


        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "RejectGroupApplication",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerId = playerId
            },
            GeneratePlayStreamEvent = true,
        },
          CloudScriptSuccess

        , CloudScriptFailure);
    }

    /// <summary>
    /// Create Applications Object in newly created Group On PlayFab 
    /// </summary>
    [ContextMenu("Upload Data")]
    public void AddUserApplicationInGroupObjects()
    {


        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "AddUserApplicationInGroupObjects",
            FunctionParameter = new
            {
                GroupId = playFabClanData.newGroupsData.Group.Id,
                Data = "[]"
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }
    /// <summary>
    /// Add any Object in Group On PlayFab for the given group id,specified key and json data 
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="key">Name of the object to add</param>
    /// <param name="data">Object to be added in json format</param>
    public void AddSpecifiedObjectInGroupObjects(string groupId, string key, string data) // there is initially a limit of max 5 objects 
    {


        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "AddSpecifiedObjectInGroupObjects",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerStars = PlayFabManager.Instance.currentMember.getStars(),
                Data = data,
                Key = key
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure); ;
    }

    /// <summary>
    /// Add any empty object in objects of group data
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="objectName">Object name to be added</param>
    public void AddObjectInGroup(string groupId, string objectName)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "AddObjectInGroup",
            FunctionParameter = new
            {
                GroupId = groupId,
                Data = "",
                ObjectName = objectName
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);

    }


    /// <summary>
    /// Group Creation Api
    /// </summary>
    /// <param name="newGroupName">The group name of the new group</param>
    public void CreateGroup(string newGroupName)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "CreateGroup",
            FunctionParameter = new { groupName = newGroupName },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);

    }

    /// <summary>
    /// Get All Groups on the server. The data is then populated in "playFabClanData.groupsData"
    /// </summary>
    public void GetAllGroups()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "makeEntityListMembershipAPICallTest",
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);

    }

    /// <summary>
    /// Testing Make Admin On Group Creation
    /// </summary>

    private void MakeAdmin()
    {
        AddTeamAdminOnGroupCreation(playFabClanData.newGroupsData.Group.Id);
    }


    /// <summary>
    /// Apply To Group From UI if CurrentGroupDataIsPopulated after ListGroupMembers api
    /// </summary>

    public void ApplyToGroup()
    {

        PlayFabManager.Instance.SendPlayerData();
        Debug.LogError("apply to group call thay che:" + playFabClanData.groupInformation.isOpen);
        if (playFabClanData.groupInformation.isOpen)
        {
            AddMemberToOpenGroup();
            return;
        }
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "ApplyToGroup",
            FunctionParameter = new
            {
                GroupId = playFabClanData.currentGroupData.Id,
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);

    }

    /// <summary>
    /// Add this player to the selected group 
    /// </summary>
    public void AddMemberToOpenGroup()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "AddMemberToOpenGroup",
            FunctionParameter = new
            {
                GroupId = playFabClanData.currentGroupData.Id,
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }





    /// <summary>
    /// Apply To Group with group id 
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    public void ApplyToGroup(string groupId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "ApplyToGroup",
            FunctionParameter = new
            {
                GroupId = groupId,
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }

    /// <summary>
    /// Get Data Object from group Objects
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="key"></param>

    public void GetSpecifiedDataObjectForTheGroup(string groupId, string key)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest


        {
            FunctionName = "GetSpecifiedDataObjectForTheGroup",
            FunctionParameter = new
            {
                GroupId = groupId,
                Key = key
            },
            GeneratePlayStreamEvent = true

        },

        result =>
        {
            //this is specific to requirement more cases can be added here for scaling up
            switch (key)
            {


                case "GroupInformation":
                    {
                        playFabClanData.groupInformation = JsonUtility.FromJson<GroupInformation>(result.FunctionResult.ToString());
                        Debug.Log(result.FunctionResult.ToString());
                    }
                    break;

                case "Applications":
                    {
                        //PlayPabUiController.Instance.teamCommanLoaderObj.SetActive(false);
                        Debug.Log(result.FunctionResult);
                        if (result.FunctionResult != null)
                        {
                            playFabClanData.MembersFromPlayfab = JsonUtility.FromJson<PlayersDataDummyListClass>("{\"groupMembers\":" + result.FunctionResult.ToString() + "}");
                            playFabClanData.LevelCoversion(playFabClanData.MembersFromPlayfab, playFabClanData.appliedMembers);

                        }
                        PopulateGroupDataAfterLoadingFromServer(playFabClanData.selectedGroupApplications, playFabClanData.appliedMembers);


                    }
                    break;

                case "Members":
                    {
                        Debug.Log(result.FunctionResult);
                        if (result.FunctionResult != null)
                        {
                            playFabClanData.MembersFromPlayfab = JsonUtility.FromJson<PlayersDataDummyListClass>("{\"groupMembers\":" + result.FunctionResult.ToString() + "}");
                            playFabClanData.LevelCoversion(playFabClanData.MembersFromPlayfab, playFabClanData.groupMembers);

                        }
                        PopulateGroupDataAfterLoadingFromServer(playFabClanData.allMembersInSelectedGroup, playFabClanData.groupMembers);


                    }
                    break;

                default:
                    {
                        Debug.Log(result.FunctionResult);

                    }
                    break;

            }
        }
        , CloudScriptFailure);
    }



    /// <summary>
    /// Leave group api call. This will be successful only if the player is in the passed group
    /// </summary>
    /// <param name="groupid">Selected Group id</param>
    /// <param name="playerId">Player which will exit the group</param>
    public void leaveGroup(string groupid, string playerId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "leaveGroup",
            FunctionParameter = new
            {
                groupId = groupid,
                userId = playerId
            },
            GeneratePlayStreamEvent = true
        },
        CloudScriptSuccess
        ,
        CloudScriptFailure
        );
    }
    /// <summary>
    /// Application data of a player is moved to Members data object or player application acception 
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="memberId">The to perform action on</param>
    public void MoveFromApplicationsToMembers(string groupId, string memberId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "MoveFromApplicationsToMembers",
            FunctionParameter = new { GroupId = groupId, ID = memberId },
            GeneratePlayStreamEvent = true
        }, result =>
        {
            //ToPromoteIfSecondMember();
            //ahiya e request ne delete karvani ne loader false karavi devi chirag.......
            Debug.LogError("move from application to member");
            //     playPabUicontroller.DeleteApplicationRequestForAdmineAccepted();
        }, error => { Debug.LogError(error.GenerateErrorReport()); });


    }


    /// <summary>
    /// To load data into GroupApplications class obj after data is loaded from the server
    /// </summary>
    /// <param name="groupApplications">GroupApplications object</param>
    /// <param name="MembersList">PlayersDataListClass object</param>
    private void PopulateGroupDataAfterLoadingFromServer(GroupApplications groupApplications, PlayersDataListClass MembersList)
    {
        foreach (MemberData members in MembersList.groupMembers)
        {

            PlayerApplication currentAppliction = groupApplications.GetEntityFromApplications(members.ID);
            if (currentAppliction != null)
            {
                currentAppliction.Entity.Key.setGroupUsername(members.GroupUsername);
                currentAppliction.Entity.Key.setLevel(members.Level);
            }


        }
        //if (uiController != null)
        //       uiController.PopulateGroupApplications(playFabClanData.selectedGroupApplications);
        // if (playPabUicontroller != null)
        //     playPabUicontroller.PopulateGroupApplications(playFabClanData.selectedGroupApplications);
    }
    /// <summary>
    /// To load data into AllMembers class obj after data is loaded from the server
    /// </summary>
    /// <param name="allMembersInTheGroup">AllMembers object</param>
    /// <param name="MembersList">PlayersDataListClass object</param>
    private void PopulateGroupDataAfterLoadingFromServer(AllMembers allMembersInTheGroup, PlayersDataListClass MembersList)
    {


        foreach (MemberData members in MembersList.groupMembers)
        {

            foreach (Member member in allMembersInTheGroup.Members)
            {

                foreach (MemberClass memberClass in member.Members)
                {
                    if (memberClass.Key.Id == members.ID)
                    {
                        memberClass.Key.MemberPersonalData.GroupUsername = members.GroupUsername;
                        memberClass.Key.MemberPersonalData.Level = members.Level;

                    }
                }

            }
        }



        //if(uiController!=null)  
        //  uiController.PopulateGroupOfCurrentPlayer();

        //if (playPabUicontroller != null)
        //    playPabUicontroller.CheckOfAllPlayerAndThisPlayerAndRemoveThisPlayer();

        // if (playPabUicontroller != null)
        //     playPabUicontroller.CrateGroupInfoPopupAllMamberInfo();

        PlayFabManager.Instance.GetPhotonDetails();
    }
    /// <summary>
    /// Add player as admin callback function
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    [ContextMenu("Add Member")]
    public void AddTeamAdminOnGroupCreation(string groupId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "addTeamAdminOnGroupCreation",
            FunctionParameter = new { GroupId = groupId },
            GeneratePlayStreamEvent = true
        }, result =>
        {
            StartCoroutine(AddDataObjects(groupId, new string[] { "GroupInformation" }));
        }, error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    /// <summary>
    /// Add Data Objects in group data with keys specified in dataobjects array. Currently there is a limit of 5 max objects
    /// </summary>
    /// <param name="groupid">Selected Group id</param>
    /// <param name="DataObjectsKeys">Array containing keys for DataObjects</param>

    private IEnumerator AddDataObjects(string groupid, string[] DataObjectsKeys)
    {

        foreach (string st in DataObjectsKeys)
        {
            string data = "";
            switch (st)
            {
                case "GroupInformation":
                    {
                        data = JsonUtility.ToJson(playFabClanData.groupInformation);
                    }
                    break;
                default:
                    {
                        data = "[]";
                    }
                    break;
            }
            AddSpecifiedObjectInGroupObjects(groupid, st, data);
            yield return new WaitForSeconds(1);
        }

    }


    /// <summary>
    /// Add this player's data such as username and level etc into group object for future references
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="memberData">MemberData object reference</param>
    private void AddPlayerApplicationDataToApplicationsObject(string groupId, MemberData memberData)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetGroupDataApplicationsOnthisUserApplication",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerTitleId = memberData.ID,
                PlayerGroupUserName = memberData.GroupUsername,
                PlayerLevel = "Level:" + memberData.Level


            },
            GeneratePlayStreamEvent = true,
        },
            CloudScriptSuccess
        , CloudScriptFailure);
    }

    /// <summary>
    /// Add member to group data object the player is added to open group
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="memberData">MemberData object of the current player</param>
    private void AddMembertoMembersDataObject(string groupId, MemberData memberData)
    {

        Debug.Log("here");

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "AddMembertoMembersDataObject",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerTitleId = PlayFabManager.Instance.currentMember.Id,
                PlayerGroupUserName = memberData.GroupUsername,
                PlayerLevel = "Level:" + memberData.Level
            },
            GeneratePlayStreamEvent = true,
        },

CloudScriptSuccess
        , CloudScriptFailure);

    }




    [ContextMenu("AddPlayerHelp")]
    /// <summary>
    /// Iterate Player Help COunter in the DataObject
    /// </summary>
    public void AddPlayerHelp()
    {
        string groupId = "";

        if (groupId == "")
            groupId = PlayFabManager.Instance.currentMember.getGroup().Id;
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "AddPlayerHelp",
            FunctionParameter = new
            {
                GroupId = groupId
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);



    }



    /// <summary>
    /// Add required roles on group after group creation
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    public void CreateRequiredRolesOnGroupCreation(string groupId)
    {

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "CreateRequiredRolesOnGroupCreation",
            FunctionParameter = new
            {
                GroupId = groupId,
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }


    /// <summary>
    /// Returns groups the player is member of
    /// </summary>


    public void ListMembershipRequestForCurrentMember()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "ListMembershipRequestForCurrentMember",
            FunctionParameter = new { },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }





    /// <summary>
    /// Returns members in a group 
    /// </summary>
    /// <param name="groupEntity">Group Class Object</param>
    public void ListGroupMembers(Group groupEntity)
    {
        playFabClanData.currentGroupData = groupEntity;
        Debug.Log(JsonUtility.ToJson(groupEntity));
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "GetGroupsMembersAPICall",
            FunctionParameter = new
            {


                groupId = groupEntity.Id,
                groupType = groupEntity.Type,
                groupString = groupEntity.TypeString
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure); ;

    }
    /// <summary>
    /// Returns applications for a group against the group id paramter
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    public void ListApplicationsForThisGroup(string groupId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "ListApplicationsForThisGroup",
            FunctionParameter = new
            {
                GroupId = groupId,
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }



    /// <summary>
    /// Returns members in the group
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="isCallFromSearch"></param>
    public void GetListGroupMembers(string groupId, bool isCallFromSearch = false)
    {


        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "GetListGroupMembers",
            FunctionParameter = new
            {
                GroupId = groupId,
            },
            GeneratePlayStreamEvent = true,
        }, result =>
        {
            playFabClanData.allMembersInSelectedGroup = JsonUtility.FromJson<AllMembers>(result.FunctionResult.ToString());
            Debug.Log("GetListGroupMembers.......:" + 1);
            if (isCallFromSearch)
            {

                //Debug.LogError("GetListGroupMembers membergroupe i not null2:" + playFabClanData.newGroupsData.Group.Id);
                GetSpecifiedDataObjectForTheGroup(playFabClanData.newGroupsData.Group.Id, "Members");
                return;
            }
            else
            {
                if (GetSelectedMembersCount() < 4 && promoteMember)
                {
                    ChangeMemberRole(playFabClanData.thisPlayerGroupData.Groups[0].Group.Id, playFabClanData.acceptedApplicationMember.Id, "members", "subLeader");
                }
                if (!string.IsNullOrEmpty(PlayFabManager.Instance.currentMember.MemberGroup.Id))
                {
                    //Debug.LogError("GetListGroupMembers membergroupe i not null0:" + PlayFabManager.Instance.currentMember.MemberGroup.Id);
                    GetSpecifiedDataObjectForTheGroup(PlayFabManager.Instance.currentMember.MemberGroup.Id, "Members");
                }
                else
                {
                    Debug.LogError("PlayFabManager.Instance.currentMember.MemberGroup.Id Missing");
                }
                return;



            }
        }, CloudScriptFailure);
    }





    /// <summary>
    /// Accept aaplication for the player
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="playerId">The player to accpet application of</param>

    public void AcceptGroupApplication(string groupId, string playerId)
    {

        playFabClanData.acceptedApplicationMember = new Key(playerId);

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "AcceptGroupApplication",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerId = playerId
            },
            GeneratePlayStreamEvent = true,
        }, result =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log(result.FunctionResult.ToString());
            }
            MoveFromApplicationsToMembers(groupId, playerId);
        }, CloudScriptFailure);
    }

    /// <summary>
    /// Change role of the player
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="playerId">Selected Group id</param>
    /// <param name="oldRole">previous role of the player</param>
    /// <param name="newRole">new role of the player</param>
    public void ChangeMemberRole(string groupId, string playerId, string oldRole, string newRole)
    {


        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "ChangeMemberRole",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerId = playerId,
                OldRoleid = oldRole,
                newRoleid = newRole
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);

    }



    /// <summary>
    /// Change role of the player
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="playerId">Selected Group id</param>
    /// <param name="oldRole">previous role of the player</param>
    /// <param name="newRole">new role of the player</param>

    public void ChangeMemberRole(string newRole)
    {

        string groupId = playFabClanData.promotionMember.MemberGroup.Id;
        string playerId = playFabClanData.promotionMember.Id;
        string oldRole = playFabClanData.promotionMember.MemberRole.RoleId;
        Debug.Log(oldRole);
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "ChangeMemberRole",
            FunctionParameter = new
            {
                GroupId = groupId,
                PlayerId = playerId,
                OldRoleid = oldRole,
                newRoleid = newRole
            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }





    /// <summary>
    /// Get count of members in a group if GetListGroupMembers is executed before 
    /// </summary>

    public int GetSelectedMembersCount()
    {
        int membersCount = 0;
        foreach (var role in playFabClanData.allMembersInSelectedGroup.Members)
        {
            membersCount += role.Members.Count;
        }
        return membersCount;
    }
    /// <summary>
    ///Checks and promotes 2nd Member in group to Leader 
    /// </summary>
    public void ToPromoteIfSecondMember()
    {
        promoteMember = true;
        GetListGroupMembers(playFabClanData.thisPlayerGroupData.Groups[0].Group.Id);
    }


    /// <summary>
    /// populate promotionmember object in playfabclandata 
    /// </summary>


    /// <summary>
    /// populate promotionmember object in playfabclandata 
    /// </summary>
    /// <param name="memberId"> Selected member id</param>
    /// <param name="role">role of the player</param>
    /// <param name="group">selected group object</param>
    public void PopulatePromotionPlayer(string memberId, Role role, Group group)
    {
        playFabClanData.promotionMember = new Key(memberId, role, group);
    }
    /// <summary>
    /// Bans the player from the group
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="memberId">Player to ban from the group</param>

    public void BanMemberFromGroup(string groupId, string memberId)
    {
        Debug.Log(groupId + " " + memberId);
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {

            FunctionName = "BlockMemberFromGroup",
            FunctionParameter = new
            {
                GroupId = groupId,
                MemberId = memberId

            },
            GeneratePlayStreamEvent = true,
        }, CloudScriptSuccess, CloudScriptFailure);
    }
    /// <summary>
    /// Fetch the group data for the required group
    /// </summary>
    /// <param name="currentPlayerid">Player id for who data is needed</param>
    public void GetCurrentPlayerDataForGroup(string currentPlayerid = "")
    {

        //Debug.LogError("GetCurrentPlayerDataForGroup:"+ currentPlayerid.Length);
        if (currentPlayerid.Length > 0)
        {

            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
            {

                FunctionName = "ListMembershipRequestForLoggedinMember",
                FunctionParameter = new
                {


                },
                GeneratePlayStreamEvent = true,
            }, CloudScriptSuccess, CloudScriptFailure);

        }
        else
        {


            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
            {

                FunctionName = "ListMembershipForAnyPlayer",
                FunctionParameter = new
                {
                    MemberId = currentPlayerid


                },
                GeneratePlayStreamEvent = true,
            }, CloudScriptSuccess, CloudScriptFailure);
        }
    }


    /// <summary>
    /// Creates shared group object on playfab with the given id and add the member for the given playfabid
    /// </summary>
    /// <param name="memberId">Title id of the player to add in the newly created object</param>
    /// <param name="Playfabid">Master id of the player</param>
    public void CreateSharedGroupObjectAndAddMember(string memberId, string Playfabid) // this is specific to the requirement where i am making a shared group object and a member(admin)
    {
        MemberDataRequestClass dummyMemberObject = new MemberDataRequestClass(PlayFabManager.Instance.currentMember.Id,
                       PlayFabManager.Instance.currentMember.MemberPersonalData.Level,
                       PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername);
        string data = "[" + JsonUtility.ToJson(dummyMemberObject) + "]";
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "CreateSharedGroupObject",
            FunctionParameter = new
            {
                Key = memberId,
                Id = Playfabid,
                Username = PlayFabManager.Instance.username,
                MembersData = data
            },
            GeneratePlayStreamEvent = true,
        }, result => { Debug.Log(result.FunctionResult); }, CloudScriptFailure);
    }

    /// <summary>
    /// Add member to shared group object for the groupId, by playfab id , it specifically means master player account id
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="Playfabid">Selected player id</param>
    public void AddMemberToSharedGroupObject(string groupId, string Playfabid)  // by playfab id , it specifically means master player account id
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "AddMemberToSharedGroupObject",
            FunctionParameter = new
            {
                Key = groupId,
                Id = Playfabid
            },
            GeneratePlayStreamEvent = true,
        }, result => { Debug.Log(result.FunctionResult); }, CloudScriptFailure);



    }


    /// <summary>
    /// Upload message to the server and add to the shared GroupObject for the given groupId.
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    /// <param name="message">Message object to add</param>
    public void UploadMessageToSharedGroupObject(string groupId, string message)
    {

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "UploadMessageToSharedGroupObject",
            FunctionParameter = new
            {
                SharedGroupId = groupId,
                Message = message

            },
            GeneratePlayStreamEvent = true,
        }, result =>
        {
            Debug.Log(result.FunctionResult);
            if (PlayFabManager.Instance.currentMember.getChatEnabled())
            {
                if (!string.IsNullOrEmpty(PlayFabManager.Instance.currentMember.MemberGroup.Id))
                {
                    Debug.LogError("msg upload pasi0 :" + PlayFabManager.Instance.currentMember.MemberGroup.Id);
                    DownloadMessagesFromSharedGroupObject(PlayFabManager.Instance.currentMember.MemberGroup.Id);
                }
                else
                {
                    Debug.LogError("msg upload pasi1 :" + playFabClanData.currentGroupData.Id);
                    DownloadMessagesFromSharedGroupObject(playFabClanData.currentGroupData.Id);
                }
            }
        }, CloudScriptFailure);




    }

    /// <summary>
    /// Download messages for the shared group data for the groupId and populate in downloadMessages attribute fo chatmanager class
    /// </summary>
    /// <param name="groupId">Selected Group id</param>
    public void DownloadMessagesFromSharedGroupObject(string groupId)
    {

        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "DownloadMessagesFromSharedGroupObject",
            FunctionParameter = new
            {
                SharedGroupId = groupId

            },
            GeneratePlayStreamEvent = true,
        }, result =>
        {
            if (result.FunctionResult != null)
            {
                if (PlayFabManager.Instance.currentMember.getChatEnabled())
                {
                    ChatManager.Instance.DownloadedMessages = JsonUtility.FromJson<MessageList>(result.FunctionResult.ToString());
                    //chirag.......
                    //Debug.LogError("callcounttttttttt :" + ChatScreenManager.callCount);
                }
            }
        }, CloudScriptFailure);




    }
    /// <summary>
    /// CallBack on Cloud Scrpit function Execution Success to populate playfabclandata objects
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    public void CloudScriptSuccess(ExecuteCloudScriptResult result)
    {

        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);

        switch (result.FunctionName)
        {

            case "GetGroupInformation":
                {
                    try
                    {
                        Debug.Log(result.FunctionResult);
                        if (result.FunctionResult.ToString().Contains("Wrong Input"))
                        {
                            Debug.LogError("Wrong InputWrong InputWrong InputWrong Input");
                        }
                        else
                        {
                            groupInfo = JsonUtility.FromJson<NewGroupInformation>(result.FunctionResult.ToString());
                        }

                        //both condition myteam in info popup.......
                        if (result.FunctionResult != null)
                        {

                        }

                    }
                    catch
                    {
                        GetGroupInformation(PlayFabManager.Instance.currentMember.MemberGroup.Id);
                    }
                }
                break;
            case "leaveGroup":
                {
                    ChatManager.Instance.GroupUpdate(PlayFabManager.Instance.currentMember.GetGroupUsername(), GroupUpdateType.UpdateType.Left);
                    Debug.Log(result.FunctionResult);
                    //ListMembershipRequestForCurrentMember();
                    //PlayFabManager.Instance.currentMember.MemberRole = new Role();
                    //PlayFabManager.Instance.currentMember.MemberGroup = new Group();
                    //refresh ui here
                    if (result.FunctionResult != null)
                    {
                    }
                }
                break;
            case "makeEntityListMembershipAPICallTest":
                {
                    playFabClanData.groupsData = JsonUtility.FromJson<GroupsData>(result.FunctionResult.ToString());
                    //if (uiController != null)
                    //    uiController.CreateButtonsBaseOnList(playFabClanData.groupsData);
                    // if (playPabUicontroller != null)
                    //     playPabUicontroller.CreateButtonsBaseOnList(playFabClanData.groupsData);
                    // //if (!playPabUicontroller.isListMemberLoaded)
                    //{
                    //    Debug.LogError("AA...............................................");
                    //    //ahiya get my group nu call karvanu che chirag.......
                    //    ListMembershipRequestForCurrentMember();
                    //}
                }
                break;
            case "RejectGroupApplication":
                {
                    //if (result.FunctionResult != null)
                    //{
                    //    playFabClanData.selectedGroupApplications = JsonUtility.FromJson<GroupApplications>(result.FunctionResult.ToString());
                    //}
                    //   playPabUicontroller.DeleteApplicationRequestForAdmineAccepted();
                    //GetSpecifiedDataObjectForTheGroup(PlayFabManager.Instance.currentMember.MemberGroup.Id, "Applications");
                    ListApplicationsForThisGroup(PlayFabManager.Instance.currentMember.getGroup().Id);
                }
                break;
            case "CreateRequiredRolesOnGroupCreation":
                {
                    AddTeamAdminOnGroupCreation(playFabClanData.newGroupsData.Group.Id);
                }
                break;
            case "CreateGroup":
                {
                    playFabClanData.newGroupsData = JsonUtility.FromJson<NewGroupData>(result.FunctionResult.ToString());
                    CreateRequiredRolesOnGroupCreation(playFabClanData.newGroupsData.Group.Id);
                    CreateSharedGroupObjectAndAddMember(playFabClanData.newGroupsData.Group.Id, PlayFabManager.Instance.PlayFabUserId);
                    Debug.LogError("create group then after get username:" + PlayFabManager.Instance.username);
                    PlayFabManager.Instance.currentMember.setGroupUsername(PlayFabManager.Instance.username);
                    PlayFabManager.Instance.SendPlayerData();
                    //if (result.FunctionResult != null)
                    //{
                    //    ChatManager.Instance.OnCreatedForOther();
                    //}
                    //     playPabUicontroller.isRefreshForNotAnyGroupInJoin = false;
                    //     playPabUicontroller.CancelInvokeForNotAnyGroup();
                    //     //GetCurrentPlayerDataForGroup(PlayFabManager.Instance.PlayFabUserId);

                    //     PlayPabUiController.Instance.Reduce100CoinToCreateTeam();
                    // 
                }
                break;
            case "GetGroupsMembersAPICall":
                {
                    Debug.LogError("iscreategroupsuccess:");
                    // if (!playPabUicontroller.isCreateGroupeSuccess)
                    // {
                    //     GetSpecifiedDataObjectForTheGroup(playFabClanData.currentGroupData.Id, "GroupInformation");
                    // }
                    //if (uiController != null)
                    //    uiController.SetText(result.FunctionResult.ToString());
                }
                break;
            case "ApplyToGroup":
                {
                    if (applyWithName)
                    {
                        Debug.LogError("ApplyToGroup0.......:" + playFabClanData.currentGroupData.Id + ":member personal data:" + PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername + ":mersonal data id:" + PlayFabManager.Instance.currentMember.MemberPersonalData.ID + ":personal data:" + PlayFabManager.Instance.currentMember.MemberPersonalData);
                        if (string.IsNullOrEmpty(PlayFabManager.Instance.currentMember.MemberPersonalData.ID))
                        {
                            PlayFabManager.Instance.currentMember.MemberPersonalData.ID = PlayFabManager.Instance.currentMember.Id;
                        }
                        Debug.LogError("ApplyToGroup1.......:" + playFabClanData.currentGroupData.Id + ":member personal data:" + PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername + ":mersonal data id:" + PlayFabManager.Instance.currentMember.MemberPersonalData.ID + ":personal data:" + PlayFabManager.Instance.currentMember.MemberPersonalData);
                        AddPlayerApplicationDataToApplicationsObject(playFabClanData.currentGroupData.Id, PlayFabManager.Instance.currentMember.MemberPersonalData);
                    }
                }
                break;
            case "AddMemberToOpenGroup":
                {
                    Debug.LogError("add member to open group:" + playFabClanData.currentGroupData.Id);
                    AddMembertoMembersDataObject(playFabClanData.currentGroupData.Id, PlayFabManager.Instance.currentMember.MemberPersonalData);
                    AddMemberToSharedGroupObject(playFabClanData.currentGroupData.Id, PlayFabManager.Instance.PlayFabUserId);
                    //   ListMembershipRequestForCurrentMember();
                    ChatManager.Instance.NewMember = true;
                    //chirag add membe open groupe after change ui.......
                    //    playPabUicontroller.ChangeUIJoinOnOpenGroup();
                }
                break;
            case "GetGroupDataApplicationsOnthisUserApplication":
                {
                    //chirag.......
                    Debug.Log(result.FunctionResult);
                    // if (playPabUicontroller != null)
                    // {
                    //     if (playPabUicontroller.teamCommanLoaderObj.activeSelf)
                    //     {
                    //         playPabUicontroller.teamCommanLoaderObj.SetActive(false);
                    //     }
                    //     playPabUicontroller.currentSelectenTeamItemController.joinButton.interactable = false;
                    //     playPabUicontroller.currentSelectenTeamItemController.joinButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Pending";
                    // }
                }
                break;
            case "SetGroupDataApplicationsOnthisUserApplication":
                {

                }
                break;
            case "ListMembershipRequestForCurrentMember":
                {
                    playFabClanData.thisPlayerGroupData = JsonUtility.FromJson<GroupsData>(result.FunctionResult.ToString());
                    Debug.LogError("ListMembershipRequestForCurrentMember....... call thay che thisPlayerGoupData.count:" + playFabClanData.thisPlayerGroupData.Groups.Count );
                    if (playFabClanData.thisPlayerGroupData.Groups.Count > 0)
                    {
                        //cancle invoke......
                        //playPabUicontroller.CancelInvokeForNotAnyGroup();
                            Debug.LogError("current meber no data load thay che0....");
                            GetCurrentPlayerDataForGroup(PlayFabManager.Instance.PlayFabUserId);
                        //    playPabUicontroller.isRefreshForNotAnyGroupInJoin = false;
                            promoteMember = false;
                            GetListGroupMembers(PlayFabManager.Instance.currentMember.MemberGroup.Id);
                            GetSpecifiedDataObjectForTheGroup(PlayFabManager.Instance.currentMember.MemberGroup.Id, "GroupData");
                            GetGroupInformation(PlayFabManager.Instance.currentMember.MemberGroup.Id);
                            //chirag.......

                    }
               
                }
                break;
            case "ListApplicationsForThisGroup":
                {
                    playFabClanData.selectedGroupApplications = JsonUtility.FromJson<GroupApplications>(result.FunctionResult.ToString());
                    GetSpecifiedDataObjectForTheGroup(PlayFabManager.Instance.currentMember.MemberGroup.Id, "Applications");
                }
                break;
            case "EditGroupInformation":
                {
                    Debug.Log(result.FunctionResult);
                    if (result.FunctionResult != null)
                    {
                        //chirag.......
                        GetGroupInformation(PlayFabManager.Instance.currentMember.MemberGroup.Id);
                    }
                   
                }
                break;
            case "ChangeGroupName":
                {
                    if (result.FunctionResult != null)
                    {
                        Debug.Log(result.FunctionResult);
                    }
                }
                break;
            case "AcceptGroupApplication":
                {
                    //ToPromoteIfSecondMember();
                    //  AddMembersToSharedGroupData(        );
                }
                break;

            case "ListMembershipRequestForLoggedinMember":
                {
                    GroupsData groupsData = JsonUtility.FromJson<GroupsData>(result.FunctionResult.ToString());
                    //Debug.LogError("ListMembershipRequestForLoggedinMember:" + groupsData.Groups.Count + ":ListMembershipRequestForLoggedinMember:"+ groupsData);
                    if (groupsData.Groups.Count > 0)
                    {
                        //Debug.Log("AM I COMING HERE");
                        //Debug.LogError("ListMembershipRequestForLoggedinMember1123:" + groupsData.Groups[0].Roles[0] + ":" + groupsData.Groups[0].Group + ":" + groupsData.Groups[0].GroupName);
                        PlayFabManager.Instance.currentMember.setRoleAndGroup(groupsData.Groups[0].Roles[0], groupsData.Groups[0].Group, groupsData.Groups[0].GroupName);

                    }
               
                }
                break;
            case "ListMembershipForAnyPlayer":
                {
                    Debug.Log(result.FunctionResult);
                }
                break;
            case "BlockMemberFromGroup":
                {
                    Debug.Log(result.FunctionResult);
                    
                }
                break;
            case "ChangeMemberRole":
                {
                    Debug.Log("Member Role Changed" + result.FunctionResult);
            
                }
                break;
            case "GetPlayerData":
                {
                    Debug.Log(result.FunctionResult);
                }
                break;
            case "AddSpecifiedObjectInGroupObjects":
                {
                    Debug.Log(result.FunctionResult);
                    //chirag.......
                    Debug.LogError("AddSpecifiedObjectInGroupObjects Load thay che");
                    GetCurrentPlayerDataForGroup(PlayFabManager.Instance.PlayFabUserId);
                }
                break;
            case "AddMembertoMembersDataObject":
                {
                    Debug.Log("AddMembertoMembersDataObject:" + result.FunctionResult);
                    GetCurrentPlayerDataForGroup(PlayFabManager.Instance.PlayFabUserId);
                    //if (result.FunctionResult != null)
                    //{
                    //    ChatManager.Instance.OnCreatedForOther();
                    //}

                    //if (playPabUicontroller.currentSelectenTeamItemController != null)
                    //{
                    //    GetListGroupMembers(playPabUicontroller.currentSelectenTeamItemController.groupId);
                    //}
                }
                break;
            case "SetPlayerLevel":
                {
                    //refresh player level on team tournamnet.......
                    PlayFabManager.Instance.GetUserReadOnlyData();
                }
                break;
                //case "MoveFromApplicationsToMembers":  /// Made CHanges Here 9/11/20
                //    {


                //        ListMembershipRequestForCurrentMember();


                //    }
                //    break;       
        }
    }


    /// <summary>
    /// Clear all data on sign out or for any other purpose
    /// </summary>
    public void ClearData()
    {
        playFabClanData = new PlayFabClanData();
    }

    //this method are clear data without group list chirag.......
    public void ClearDataWithoutGroupList()
    {
        playFabClanData.newGroupsData = new NewGroupData();
        //playFabClanData.groupInformation = new GroupInformation();
        //playFabClanData.currentGroupData = new Group();
        playFabClanData.thisPlayerGroupData.Groups.Clear();
        playFabClanData.selectedGroupApplications = new GroupApplications();
        playFabClanData.allMembersInSelectedGroup = new AllMembers();
        playFabClanData.acceptedApplicationMember = new Key();
        playFabClanData.promotionMember = new Key();
        playFabClanData.groupMembers = new PlayersDataListClass();
        playFabClanData.appliedMembers = new PlayersDataListClass();
        playFabClanData.MembersFromPlayfab = new PlayersDataDummyListClass();
        MasterPlayerids.Clear();
        //groupInfo = new NewGroupInformation();
    }
    /// <summary>
    /// cloud script function failure callback
    /// </summary>
    public void CloudScriptFailure(PlayFabError error)
    {

        Debug.Log(error.Error);
    }
}


