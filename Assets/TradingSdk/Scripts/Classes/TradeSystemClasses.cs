using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ClanSystem.VirtualCurrency.CloudResponse
{
	[System.Serializable]
	public class Response
	{
		public string CO;//coins
		public string LF;//life
		public string EN;//life
		public string HT;//coins
		public string SW;//life
		public string TM;//life
		public string B1;//life
		public string B2;//life
		public string B3;//life
		public string DT;//life

	}

	[System.Serializable]
	public class ResponseContent
	{
		public Response response;
	}
}

namespace ClanSystem.AwardLife.CloudResponse
{
	[System.Serializable]
	public class ResponseContent
	{
		public Response response;
	}

	[System.Serializable]
	public class Response
	{
		public string VirtualCurrency;
		public int BalanceChange;
		public int Balance;
	}
}

namespace ClanSystem.ConsumeItem.CloudResponse
{
	[System.Serializable]
	public class Response
	{
		public string VirtualCurrency;
		public int Balance;
	}

	[System.Serializable]
	public class ResponseContent
	{
		public Response response;
	}
}



namespace CloudResponse.SpinWheel
{
	[System.Serializable]
	public class Node
	{
		public string ResultItemType;
		public string ResultItem;
		public float Weight;
	}

	[System.Serializable]
	/// <summary>
	/// Playfab spinwheel data class 
	/// </summary>
	public class SpinWheel
	{
		public long CatalogVersion;
		public string TableId;
		public List<Node> Nodes;
	}
	[Serializable]
	public class TimerClass
	{
		public bool needsToRecharge = false;
		public int secondsToRecharge = 0;
	}
}


[System.Serializable]
public class InventoryMessageItem
{
	public string senderName;
	public string message = "sent you a life!";
	public int messageNumber;
}

[System.Serializable]
public class InventoryMessages
{
	public List<InventoryMessageItem> messages;
}

[System.Serializable]
public class LifeRequestItem
{
	public string name;
	public string playfabID;
	public int lifeReceived;
	public List<string> unallowedMembers;
}

[System.Serializable]
/// <summary>
/// LifeRequestItem list class
/// </summary>
public class LifeRequest
{
	public List<LifeRequestItem> lifeRequests = new List<LifeRequestItem>();
}



// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
[System.Serializable]
/// <summary>
/// player bonuslifetime class.
/// /// </summary>
public class LifeTimer
{
	public string startTime;
	public string endTime;
	public bool useBonusLife;

}
[System.Serializable]

public class VipTimer
{
	public string startTime;
	public string endTime;
	public bool canUseVip;

}
