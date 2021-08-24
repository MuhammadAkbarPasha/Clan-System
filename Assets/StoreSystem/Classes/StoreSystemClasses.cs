using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace StoreSystem
{
    /// <summary>
    /// List of DiscountedProduct class
    /// </summary>
    [System.Serializable]
    public class DiscountedProducts
    {
        public List<DiscountedProduct> discountedProducts = new List<DiscountedProduct>();
    }


    /// <summary>
    /// Discounted product details data class
    /// </summary>
    [System.Serializable]
    public class DiscountedProduct
    {
        public string id;
        public string realProduct;
        public bool isEligible = false;
        public bool hasBought;
        public int totalTime;
        public string endTime;
        public string timeLeft;
        public string startTime;
        public int levelRequired;
        public DateTime startTime_ts;
        public DateTime endTime_ts;
        public TimeSpan timeLeft_ts;
        public List<int> requiredLevels = new List<int>();
        public bool isRestart;
        public string displayName;
        public string cloudScriptFunction;
    }


    /// <summary>
    /// Single gift item data class
    /// </summary>
    [System.Serializable]
    public class Item
    {
        public string itemId;
        public string itemName;
        public int number;
    }

    /// <summary>
    /// Item or product custom data class. This contains product related data such as messages, graphical references or various prices
    /// </summary>
    [System.Serializable]
    public class CustomData
    {
        public string offerTitle;
        public string bgColor;
        public string midBgImage;
        public string discountImage;
        public List<Item> items = new List<Item>();
        public string originalPrice;
        public string offerPrice;
        public int PercentageOff;
        public int timer; // this is only needed in the case of seasonal offer.
    }


    /// <summary>
    /// Virtual currency refernce class fo the product
    /// </summary>
    [System.Serializable]
    public class VirtualCurrencyPrices
    {
        public int RM;

    }


    /// <summary>
    /// Single store data class
    /// </summary>
    [System.Serializable]
    public class Store
    {
        public string ItemId;
        public VirtualCurrencyPrices VirtualCurrencyPrices;
        public string DisplayName;
        public CustomData CustomData;
    }

    /// <summary>
    /// Store data request response data class
    /// </summary>
    [System.Serializable]
    public class StoreResponse
    {
        public List<Store> Store;
        public string StoreId;
        public string CatalogVersion;

    }


    /// <summary>
    /// Class of data received in payload of purchase. This class contains purchase information data 
    /// </summary>
    public class JsonData
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
    }


    /// <summary>
    /// Payload data class
    /// </summary>
    public class PayloadData
    {
        public JsonData JsonData;

        // JSON Fields, ! Case-sensitive
        public string signature;
        public string json;
        /// <summary>
        /// Convert json received to JsonData object and assign to class member 
        /// </summary>
        /// <param name="json">JSON format data</param>
        /// <returns>PayloadData object converted from json</returns>
        public static PayloadData FromJson(string json)
        {
            var payload = JsonUtility.FromJson<PayloadData>(json);
            payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
            return payload;
        }
    }


    /// <summary>
    /// Google purchase data class. This is used for android platform only
    /// </summary>
    public class GooglePurchase
    {
        public PayloadData PayloadData;

        // JSON Fields, ! Case-sensitive
        public string Store;
        public string TransactionID;
        public string Payload;

        /// <summary>
        /// Convert json received to JsonData object and assign to class member 
        /// </summary>
        /// <param name="json">JSON format data</param>
        /// <returns>GooglePurchase object converted from json</returns>
        public static GooglePurchase FromJson(string json)
        {
            var purchase = JsonUtility.FromJson<GooglePurchase>(json);
            purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
            return purchase;
        }
    }


    /// <summary>
    /// Receipt data class
    /// </summary>
    public class Receipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;
        public Receipt()
        {
            Store = TransactionID = Payload = "";
        }
        public Receipt(string store, string transactionID, string payload)
        {
            Store = store;
            TransactionID = transactionID;
            Payload = payload;
        }
    }

}