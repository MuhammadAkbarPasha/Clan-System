using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
public class TradeSystemController : MonoBehaviour
{
    #region ClassStaticInstance
    public static TradeSystemController Instance;
    #endregion
    #region ReferencesToResponses
    /// <summary>
    /// Life Received Data
    /// </summary>
    /// <returns></returns>
    public InventoryMessages inventoryMessages = new InventoryMessages(); 

    /// <summary>
    /// Data Received on Consuming Life
    /// </summary>
    /// <returns></returns>
    public ClanSystem.ConsumeItem.CloudResponse.ResponseContent consumeItemResponse = new ClanSystem.ConsumeItem.CloudResponse.ResponseContent(); 
    /// <summary>
    ///  Response on Giving life to another player
    /// </summary>
    /// <returns></returns>
    public ClanSystem.AwardLife.CloudResponse.ResponseContent awardResponse = new ClanSystem.AwardLife.CloudResponse.ResponseContent(); 
    /// <summary>
    /// Response of loading virtual currency
    /// </summary>
    /// <returns></returns>
    public ClanSystem.VirtualCurrency.CloudResponse.ResponseContent currencyResponse = new ClanSystem.VirtualCurrency.CloudResponse.ResponseContent(); 
    /// <summary>
    /// Response of creating life request
    /// </summary>
    /// <returns></returns>
    public LifeRequest lifeResponse = new LifeRequest(); 
    /// <summary>
    /// The message number currently being processed 
    /// </summary>
    public int messageNumber;
    #endregion
    public void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// Load Virtual Currency For The Logged In Player
    /// </summary>
    internal void LoadVirtualCurrency()
    {
        ExecuteCloudScriptRequest removeUserLifeMessage = new ExecuteCloudScriptRequest
        {
            FunctionName = "loadVirtualCurrency"
        };

        PlayFabClientAPI.ExecuteCloudScript(removeUserLifeMessage, OnSuccessfulResponse, OnFailedResponse);

    }
    /// <summary>
    /// Delete a life message from the server when it has been got by the player and update the virtual currency
    /// </summary>
    /// <param name="_messageNumber">Message index to delete</param>
    internal void DeleteUserEnergyMessage(int _messageNumber)
    {
        ExecuteCloudScriptRequest removeUserLifeMessage = new ExecuteCloudScriptRequest
        {
            FunctionName = "removeUserLifeMessage",
            FunctionParameter = new
            {
                messageNumber = _messageNumber
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(removeUserLifeMessage, OnSuccessfulResponse, OnFailedResponse);
    }
    /// <summary>
    /// Get previous life messages logged in player
    /// </summary>
    internal void GetUserLifeMessages()
    {
        ExecuteCloudScriptRequest getUserLifeMessages = new ExecuteCloudScriptRequest
        {
            FunctionName = "getUserLifeMessages"
        };
    }




    /// <summary>
    /// Award life to a player who has requested for life, accessing through awardToPlayerID, GroupUsername of the player who is giving a life
    /// </summary>
    /// <param name="awardToPlayerID"></param>
    /// <param name="username"></param>
    /// <param name="groupID"></param>
    /*internal void AwardEnergyContainer(string awardToPlayerID, string username, string groupID)
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "awardEnergy",
            FunctionParameter = new
            {
                ToPlayerID = awardToPlayerID,
                OtherPlayerUsername = username *//*your groupusername*//*,
                GroupID = groupID
            }
        
        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    }
*/
    /// <summary>
    /// Award the selected player with life
    /// </summary>
    /// <param name="awardToPlayerID">Selected player id</param>
    /// <param name="username">Username to show which player gave the life</param>
    /// <param name="groupID">Players group reference</param>
    internal void AwardLifeContainer(string awardToPlayerID, string username, string groupID)
    {
        ExecuteCloudScriptRequest awardLifeContainer = new ExecuteCloudScriptRequest
        {
            FunctionName = "awardLife",
            FunctionParameter = new
            {
                ToPlayerID = awardToPlayerID,
                OtherPlayerUsername = username /*your groupusername*/,
                GroupID = groupID
            }

        };

        PlayFabClientAPI.ExecuteCloudScript(awardLifeContainer, OnSuccessfulResponse, OnFailedResponse);
    }


    /// <summary>
    /// Use an energy from received life messages and then delete that message
    /// </summary>
    /*internal void ConsumeEnergy()
    {
        ExecuteCloudScriptRequest consumeLife = new ExecuteCloudScriptRequest
        {
            FunctionName = "consumeEnergy"
        };

        PlayFabClientAPI.ExecuteCloudScript(consumeLife, OnSuccessfulResponse, OnFailedResponse);
    }
*/


    /// <summary>
    /// Use an Life from received life messages and then delete that message
    /// </summary>

    internal void ConsumeLife()
    {
        ExecuteCloudScriptRequest consumeLife = new ExecuteCloudScriptRequest
        {
            FunctionName = "consumeLife"
        };

        PlayFabClientAPI.ExecuteCloudScript(consumeLife, OnSuccessfulResponse, OnFailedResponse);
    }

    /* /// <summary>
     /// Generate a life request for the specific group id and playerid
     /// </summary>
     /// <param name="_groupID"></param>
     /// <param name="_playerID"></param>
     internal void CreateEnergyRequest(string _groupID, string _playerID)
     {
         Debug.Log(_groupID);
         ExecuteCloudScriptRequest createGroupData = new ExecuteCloudScriptRequest
         {
             FunctionName = "createEnergyRequest",
             FunctionParameter = new
             {
                 groupID = _groupID,
                 myGroupName = PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername
             }
         };

         PlayFabClientAPI.ExecuteCloudScript(createGroupData, OnSuccessfulResponse, OnFailedResponse);
     }
 */





    /// <summary>
    /// Generate a life request for the specific group id and playerid
    /// </summary>
    /// <param name="_groupID">Group this player belongs to</param>
    /// <param name="_playerID">The player for which life request is to be created</param>
    internal void CreateLifeRequest(string _groupID, string _playerID)
    {
        Debug.Log(_groupID);
        ExecuteCloudScriptRequest createGroupData = new ExecuteCloudScriptRequest
        {
            FunctionName = "createLifeRequest",
            FunctionParameter = new
            {
                groupID = _groupID,
                myGroupName = PlayFabManager.Instance.currentMember.MemberPersonalData.GroupUsername
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(createGroupData, OnSuccessfulResponse, OnFailedResponse);
    }
    /// <summary>
    /// Get all previous life requests from the server for specified groupid
    /// </summary>
    /// <param name="_groupID">Slected Group id</param>
    internal void GetEnergyRequests(string _groupID)
    {
        ExecuteCloudScriptRequest getLifeRequests = new ExecuteCloudScriptRequest
        {
            FunctionName = "getEnergyRequests",
            FunctionParameter = new
            {
                groupID = _groupID
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(getLifeRequests, OnSuccessfulResponse, OnFailedResponse);
    }
    /// <summary>
    /// Get all previous life requests from the server for specified groupid
    /// </summary>
    /// <param name="_groupID">Slected Group id</param>
    internal void GetLifeRequests(string _groupID)
    {
        ExecuteCloudScriptRequest getLifeRequests = new ExecuteCloudScriptRequest
        {
            FunctionName = "getLifeRequests",
            FunctionParameter = new
            {
                groupID = _groupID
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(getLifeRequests, OnSuccessfulResponse, OnFailedResponse);
    }
    /// <summary>
    /// callback on successful response from the server
    /// </summary>
    /// <param name="result">ExecuteCloudScriptResult object</param>
    /// 
    public void OnSuccessfulResponse(ExecuteCloudScriptResult result)
    {
        Debug.Log("Successfully executed " + result.FunctionName);
        Debug.Log(result.FunctionResult);
        Debug.Log("data here" + result.CustomData);

        if (result.FunctionName == "loadVirtualCurrency")
        {
            if (result.FunctionResult == null)
                LoadVirtualCurrency();
            try
            {
                currencyResponse = JsonUtility.FromJson<ClanSystem.VirtualCurrency.CloudResponse.ResponseContent>(result.FunctionResult.ToString());
                PlayFabManager.Instance.setCurrency(currencyResponse.response);
            }
            catch
            {
                return;
            }
        }
        if (result.FunctionName == "awardLife")
        {
            ClanSystemController.Instance.AddPlayerHelp();
            awardResponse = JsonUtility.FromJson<ClanSystem.AwardLife.CloudResponse.ResponseContent>(result.FunctionResult.ToString());
            LoadVirtualCurrency();
            if (result.FunctionResult != null)
                ChatManager.Instance.sendEnergyRequest();
        }
        if (result.FunctionName == "awardEnergy")
        {
            Debug.Log(result.FunctionResult.ToString());
            awardResponse = JsonUtility.FromJson<ClanSystem.AwardLife.CloudResponse.ResponseContent>(result.FunctionResult.ToString());
            LoadVirtualCurrency();
            if (result.FunctionResult != null)
                ChatManager.Instance.sendEnergyRequest();
        }

        if (result.FunctionName == "consumeLife")
        {
            consumeItemResponse = JsonUtility.FromJson<ClanSystem.ConsumeItem.CloudResponse.ResponseContent>(result.FunctionResult.ToString());

            DeleteUserEnergyMessage(messageNumber);




        }
        if (result.FunctionName == "consumeEnergy")
        {
            consumeItemResponse = JsonUtility.FromJson<ClanSystem.ConsumeItem.CloudResponse.ResponseContent>(result.FunctionResult.ToString());

            DeleteUserEnergyMessage(messageNumber);




        }

        if (result.FunctionName == "getUserLifeMessages")
        {
            if (result.FunctionResult != null)
            {
                inventoryMessages = JsonUtility.FromJson<InventoryMessages>(result.FunctionResult.ToString());
            }

        }

        if (result.FunctionName == "removeUserLifeMessage")
        {
            inventoryMessages = JsonUtility.FromJson<InventoryMessages>(result.FunctionResult.ToString());
            LoadVirtualCurrency();
            GetUserLifeMessages();


            //chirag ahiya get energy thay hoy e batavani che and vfx play karvana che.......
        }
        if (result.FunctionName == "createLifeRequest")
        {
            if (result.FunctionResult != null)
            {
                ChatManager.Instance.sendEnergyRequest();
            }
        }

        if (result.FunctionName == "getLifeRequests")
        {
            if (result.FunctionResult == null)
                return;

            lifeResponse = JsonUtility.FromJson<LifeRequest>(result.FunctionResult.ToString());
        }

    }
    /// <summary>
    /// Callback on failed response from the server
    /// </summary>
    /// <param name="error">PlayFabError object</param>
    public void OnFailedResponse(PlayFabError error)
    {

        Debug.Log(error.Error);
    }

    /// <summary>
    /// clear prvious responses for signout
    /// </summary>
    public void clearData()
    {
        inventoryMessages = new InventoryMessages();
        consumeItemResponse = new ClanSystem.ConsumeItem.CloudResponse.ResponseContent();
        awardResponse = new ClanSystem.AwardLife.CloudResponse.ResponseContent();
        currencyResponse = new ClanSystem.VirtualCurrency.CloudResponse.ResponseContent();
        lifeResponse = new LifeRequest();
    }


}
