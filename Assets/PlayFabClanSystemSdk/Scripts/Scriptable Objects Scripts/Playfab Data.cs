using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(menuName ="Playfab/New PlayFab Data")]
public class PlayfabData : ScriptableObject
{

    public new string name;

    public GroupsData groupsData;
    public NewGroupData newGroupsData;
    public Group currentGroupData;
    public GroupsData thisPlayerGroupData;
    public GroupApplications selectedGroupApplications;



}
