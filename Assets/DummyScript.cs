using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GroupMembersData 
{


  public  Dictionary<string , string > GroupMembers = new Dictionary<string, string>();

}


[System.Serializable]
public class GroupMember
{
 public   string Id;
    public string Username;

    public GroupMember(string id,string username) 
    {
     this.Id = id;
        this.Username = username;
    
    }

}

public class DummyScript : MonoBehaviour
{
    // Start is called before the first frame update


    public Dictionary<string, string> GroupMembers; 

    void Start()
    {

       GroupMembers = new Dictionary<string, string>();
       GroupMembers.Add("1","asdfff");
       GroupMembers.Add("2",  "asdfff");
       GroupMembers.Add("3",  "asdfff");
       GroupMembers.Add("4",  "asdfff");
       GroupMembers.Add("5",  "asdfff");


        Debug.Log(JsonUtility.ToJson(GroupMembers  ).ToString()  );
     //  Debug.Log( GroupMembers["1" );







    }


}
