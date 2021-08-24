using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class GF_SaveLoad {

	public static void SaveProgress(){
		string saveDataHashed = JsonUtility.ToJson(SaveData.Instance, true);
		string encryptedData = EncryptDecrypt(saveDataHashed);
		SaveData.Instance.hashOfSaveData = HashGenerator(encryptedData);
		File.WriteAllText (GetSavePath (), encryptedData);
	
	
	
	}

	private static SaveData SaveObjectCreator(){
		SaveData CheckSave = new SaveData (SaveData.Instance.Username, SaveData.Instance.Password);
		return CheckSave;
	}

	public static void LoadProgress(){
		if (File.Exists (GetSavePath ())) {
			string fileContent = File.ReadAllText (GetSavePath());
			fileContent = EncryptDecrypt(fileContent);
			Debug.Log(fileContent);
			JsonUtility.FromJsonOverwrite (fileContent, SaveData.Instance);

			/*#if !UNITY_EDITOR
			//File tampering checks
			if ((HashGenerator (SaveObjectJSON()) != SaveData.Instance.hashOfSaveData)) {
				SaveData.Instance = null;
				SaveData.Instance = new SaveData();
				DeleteProgress ();
				SaveProgress ();
				Debug.LogWarning ("Save file modification detected, Resetting your progress !");
			}
			#endif*/

			Debug.Log ("Game Load Successful --> "+GetSavePath ());
			PlayFabManager.Instance.LoginWithPlayFab(SaveData.Instance.Username, SaveData.Instance.Password);
		} else {
			Debug.Log ("New Game Creation Successful --> "+GetSavePath ());
			return;
		
		}
	}

	public static string HashGenerator(string saveContent){
		SHA256Managed crypt = new SHA256Managed ();
		string hash = string.Empty;
		byte[] crypto = crypt.ComputeHash (Encoding.UTF8.GetBytes(saveContent), 0, Encoding.UTF8.GetByteCount(saveContent));
		foreach(byte bit in crypto){
			hash += bit.ToString ("x2");
		}
		return hash;
	}
	public static int key = 129;
	public static string EncryptDecrypt(string textToEncrypt)
	{
		StringBuilder inSb = new StringBuilder(textToEncrypt);
		StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
		char c;
		for (int i = 0; i < textToEncrypt.Length; i++)
		{
			c = inSb[i];
			c = (char)(c ^ key);
			outSb.Append(c);
		}
		return outSb.ToString();
	}
	public static void DeleteProgress(){

		SaveData.Instance.Password = "";
		SaveData.Instance.Username = "";

		if (File.Exists (GetSavePath ())) {
			File.Delete (GetSavePath());
		}
	}

	private static string GetSavePath(){
		return Path.Combine(Application.persistentDataPath,"SavedGame.json");
	}

	public static bool CheckIfFileExists() 
	{

		return File.Exists(GetSavePath());


	}



}
