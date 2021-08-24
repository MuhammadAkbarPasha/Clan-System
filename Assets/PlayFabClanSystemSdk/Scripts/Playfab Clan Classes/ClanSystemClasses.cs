using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Class of Country
/// </summary>
public class CountryClass
{
    
    public string CountryName;
    public string CountryCode;
    public string CountryLevel;


}


[System.Serializable]
/// <summary>
/// Playfab Class of Group. Containing all the required information of the group.
/// </summary>
public class Group
{
    public string Id;
    public string Type;
    public string TypeString;
    public string GroupName;

    public void setGroupName(string groupName)
    {
        this.GroupName = groupName;
    }

    public string getGroupName()
    {
        return this.GroupName;
    }

    public Group(string id, string ttype, string typestring)
    {
        this.Id = id;
        this.Type = ttype;
        this.TypeString = typestring;

    }

    public Group()
    {

    }
}

[System.Serializable]
/// <summary>
/// Class of the Group roles. Class data members include Role name and Id
/// </summary>
public class Role
{
    public string RoleName;
    public string RoleId;


    public Role(string roleId, string roleName)
    {
        this.RoleId = roleId;
        this.RoleName = roleName;

    }
    public Role()
    {

    }

}

[System.Serializable]
/// <summary>
/// Class containg both Group object and list of all possible Role objects
/// </summary>
public class Groups
{
    public string GroupName;
    public Group Group;
    public int ProfileVersion;
    public List<Role> Roles;

}

[System.Serializable]
/// <summary>
/// Class containing all the groups data
/// </summary>
public class GroupsData
{
    public List<Groups> Groups = new List<Groups>();

}
/// <summary>
/// Basic information of Group class
/// </summary>
[System.Serializable]
public class NewGroupData
{
    public string GroupName;
    public Group Group;
    public string MemberRoleId;
    public string AdminRoleId;
    public string Created;
    public int ProfileVersion;
}

[System.Serializable]
/// <summary>
/// Group data of the player 
/// </summary>
public class Key
{
    public string Id;
    public string Type;
    public string TypeString;
    public MemberData MemberPersonalData;

    public Role MemberRole;
    public Group MemberGroup = new Group();



    public void setGroupUsername(string groupUsername)
    {
        this.MemberPersonalData.GroupUsername = groupUsername;

    }
    public void setLevel(int level)
    {
        this.MemberPersonalData.Level = level;

    }

    public string GetGroupUsername()
    {
        return this.MemberPersonalData.GroupUsername;

    }
    public string GetLevel()
    {
        return this.MemberPersonalData.Level.ToString();

    }

    public int GetLevelInt()
    {
        return this.MemberPersonalData.Level;

    }



    public int getStars()
    {
        return this.MemberPersonalData.Stars;

    }
    public bool getChatEnabled()
    {
        return this.MemberPersonalData.ChatEnabled;

    }
    public void setChatEnabled(bool v)
    {
        this.MemberPersonalData.ChatEnabled = v;

    }

    public void setMemberRole(Role memberRole)
    {
        this.MemberRole = memberRole;

    }
    public void setMemberGroup(Group memberGroup)
    {
        this.MemberGroup = memberGroup;


    }
    public void setRoleAndGroup(Role memberRole, Group memberGroup, string groupName)
    {
        this.MemberGroup = memberGroup;
        this.MemberRole = memberRole;
        this.MemberGroup.setGroupName(groupName);
    }


    public void setTitleId(string id, string type = "title_player_account", string typeString = "title_player_account")
    {
        this.Id = id;
        this.Type = type;
        this.TypeString = typeString;

    }




    public Key(string id, string type = "title_player_account", string typeString = "title_player_account")
    {
        this.Id = id;
        this.Type = type;
        this.TypeString = typeString;


    }
    public Key()
    {

    }
    public Key(string id, Role role, Group group, MemberData memberData, string type = "title_player_account", string typeString = "title_player_account")
    {

        this.Id = id;
        this.MemberRole = role;
        this.MemberGroup = group;
        this.Type = type;
        this.TypeString = typeString;
        this.MemberPersonalData = memberData;

    }

    public Key(string id, Role role, Group group, string username, string playerTitleid, int level = 0, string type = "title_player_account", string typeString = "title_player_account")
    {

        this.Id = id;
        this.MemberRole = role;
        this.MemberGroup = group;
        this.Type = type;
        this.TypeString = typeString;
        this.MemberPersonalData = new MemberData(playerTitleid, username, level, 0);

    }







    public Key(string id, Role role, Group group, string type = "title_player_account", string typeString = "title_player_account")
    {

        this.Id = id;
        this.MemberRole = role;
        this.MemberGroup = group;
        this.Type = type;
        this.TypeString = typeString;

    }

    public Key(Role role, Group group, string type = "title_player_account", string typeString = "title_player_account")
    {

        this.MemberRole = role;
        this.MemberGroup = group;
        this.Type = type;
        this.TypeString = typeString;

    }



    public Key(string id, MemberData memberData, string type = "title_player_account", string typeString = "title_player_account")
    {

        this.Id = id;
        this.MemberPersonalData = memberData;
        this.Type = type;
        this.TypeString = typeString;

    }


    public Key(string id, string playerTitleid, int level, int exp, int reqExp, int stars, string type = "title_player_account", string typeString = "title_player_account")
    {

        this.Id = id;
        this.MemberPersonalData = new MemberData(playerTitleid, level, exp, reqExp, stars);
        this.Type = type;
        this.TypeString = typeString;

    }

    public Role getRole()
    {
        return MemberRole;

    }
    public Group getGroup()
    {
        return MemberGroup;

    }
    public MemberData getMemberData()
    {
        return MemberPersonalData;

    }

}

[System.Serializable]
/// <summary>
/// Playfab MasterPlayer account data
/// </summary>
public class MasterPlayerAccount
{
    public string Id;
    public string Type;
    public string TypeString;

}

[System.Serializable]
/// <summary>
/// Class holding MasterPlayerAccount data
/// </summary>
public class Lineage
{
    public MasterPlayerAccount master_player_account;

}

[System.Serializable]
/// <summary>
/// Playfab default Entity class
/// </summary>
public class Entity
{
    public Key Key;
    public Lineage Lineage;

}

[System.Serializable]
/// <summary>
/// Player's Group application class
/// </summary>
public class PlayerApplication
{
    public Group Group;
    public Entity Entity;
    public string Expires;

}

[System.Serializable]
/// <summary>
/// Group Applications holder class
/// </summary>
public class GroupApplications
{
    public List<PlayerApplication> Applications = new List<PlayerApplication>();




    public PlayerApplication GetEntityFromApplications(string id)
    {

        if (Applications.Exists((obj) => obj.Entity.Key.Id == id))
        {

            return Applications.Find((obj) => obj.Entity.Key.Id == id);


        }
        else
        {

            return null;
        }

    }

}







[System.Serializable]
/// <summary>
/// Player group data
/// </summary>
public class MemberClass
{
    public Key Key;
    public Lineage Lineage;
    public MemberData playerData;
}
[System.Serializable]
/// <summary>
/// Group same role members data
/// </summary>
public class Member
{
    public string RoleName;
    public string RoleId;
    public List<MemberClass> Members = new List<MemberClass>();

}
[System.Serializable]
/// <summary>
/// All members in group holder
/// </summary>
public class AllMembers
{
    public List<Member> Members = new List<Member>();

}
[System.Serializable]
/// <summary>
/// Group members data
/// </summary>
public class MemberData
{
    public string ID;
    public int Level;
    public int Experience = 0;
    public int RequiredExperience = 64;
    public int Stars = 0;
    public string GroupUsername = "";
    public int TotalHelps;
    public bool ChatEnabled = true;

    //constructor for when data is loaded from playfab
    public MemberData(string id, string groupUserName, int level, int helps)
    {
        this.ID = id; // player title id
        this.GroupUsername = groupUserName;
        this.Level = level;
        this.TotalHelps = helps;
    }
    //constructor when user creates account
    public MemberData(string playerTitleid, int level, int exp, int reqExp, int stars)
    {

        this.ID = playerTitleid; // player title id username
        this.Level = level;
        this.Experience = exp;
        this.RequiredExperience = reqExp;
        this.Stars = stars;
    }



}
/// <summary>
/// Group Member request class
/// </summary>
public class MemberDataRequestClass
{


    public string ID;
    public string Level;
    public string GroupUsername;



    public MemberDataRequestClass(string id, int level, string groupUsername)
    {
        this.ID = id;
        this.Level = "Level:" + level;
        this.GroupUsername = groupUsername;



    }


}

[System.Serializable]
/// <summary>
/// Clan classes objects holder
/// </summary>
public class PlayFabClanData
{

    public GroupsData groupsData = new GroupsData();
    public NewGroupData newGroupsData = new NewGroupData();

    public GroupInformation groupInformation = new GroupInformation();
    public Group currentGroupData = new Group(); // the group player has clicked on 
    public GroupsData thisPlayerGroupData = new GroupsData();
    public GroupApplications selectedGroupApplications = new GroupApplications();
    public AllMembers allMembersInSelectedGroup = new AllMembers();
    public Key acceptedApplicationMember = new Key();
    public Key promotionMember = new Key();
    public PlayersDataListClass groupMembers = new PlayersDataListClass();
    public PlayersDataListClass appliedMembers = new PlayersDataListClass();
    public PlayersDataDummyListClass MembersFromPlayfab = new PlayersDataDummyListClass();

    public void LevelCoversion(PlayersDataDummyListClass dummyMembers, PlayersDataListClass actualMembers)
    {
        actualMembers.groupMembers = new List<MemberData>();
        foreach (MemberDataDummy dummy in dummyMembers.groupMembers)
        {

            actualMembers.groupMembers.Add(new MemberData(dummy.ID, dummy.GroupUsername, LevelConversion(dummy.Level), dummy.TotalHelps));

        }


    }
    public int LevelConversion(string dummyLevel)
    {
        //   return 0;
        //Debug.Log(dummyLevel);
        return int.Parse(dummyLevel.Split(':')[1]);
    }



}


[System.Serializable]
/// <summary>
/// Member data list holder
/// </summary>
public class PlayersDataListClass
{
    public List<MemberData> groupMembers = new List<MemberData>();


}

[System.Serializable]
public class PlayersDataDummyListClass
{
    public List<MemberDataDummy> groupMembers = new List<MemberDataDummy>();


}


[System.Serializable]
public class MemberDataDummy
{


    public string ID;
    public string Level;
    public string GroupUsername;
    public int TotalHelps;


}




[System.Serializable]
/// <summary>
/// Group information class
/// </summary>
public class GroupInformation
{


    public string ImageId = "0";
    public string Description;
    public int RequiredLevel;
    public bool isOpen = true;

    public void setRequiredLevel(int requiredLevel = 1)
    {
        this.RequiredLevel = requiredLevel;

    }
    public int getRequiredLevel(int requiredLevel)
    {
        return requiredLevel;

    }
    public void setIsOpen(bool isOpen)
    {
        this.isOpen = isOpen;

    }

    public bool getIsOpen()
    {

        return isOpen;
    }

    public void setImageId(string id)
    {

        this.ImageId = id;
    }
    public void setDescription(string desc)
    {
        this.Description = desc;


    }
    public string getImageId()
    {
        return this.ImageId;


    }
    public string getDesciption()
    {
        return this.Description;


    }


}


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
[System.Serializable]
/// <summary>
/// Weekly energies data
/// </summary>
public class WeeklyEnergies
{
    public string ResetDate;
    public int TotalEnergies;
}
[System.Serializable]
public class NewGroupInformation
{
    public string ImageId;
    public string Description;
    public string GroupType;
    public int MemberCount;
    public int TotalStars;
    public WeeklyEnergies WeeklyEnergies;
    public int RequiredLevel;

    public string getDescription()
    {
        return this.Description;
    }

    public string getImageId()
    {
        return this.ImageId;
    }
    public string getGroupType()
    {
        return this.GroupType;
    }

    public int getMemberCount()
    {
        return this.MemberCount;
    }

    public int getRequiredLevel()
    {
        return this.RequiredLevel;
    }

}

