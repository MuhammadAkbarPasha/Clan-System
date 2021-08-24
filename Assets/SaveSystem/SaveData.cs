using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveData:MonoBehaviour{

	public static SaveData Instance;
	public string Username;
	public string Password;
	public string hashOfSaveData;
	public SaveData(string username, string password)
	{
		this.Username = username;
		this.Password = password;
	}
	public SaveData() { }



	public void Awake() 
	{

		Instance =this;
	
	}
	
	

}