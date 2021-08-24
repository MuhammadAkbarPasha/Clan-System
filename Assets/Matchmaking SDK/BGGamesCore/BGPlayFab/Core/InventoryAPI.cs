using Chamoji.Social;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chamoji.Social {
    public static class InventoryAPI {

        /// <summary>
        ///     Local cache for current PlayFab player's ßinventory instances.
        /// </summary>
        static List<ItemInstance> inventory;

        /// <summary>
        ///     Local cache for current PlayFab player's virtual currencies.
        /// </summary>
        static Dictionary<string, int> virtualCurrency;

        /// <summary>
        ///     Local cache for current PlayFab player's virtual currency recharge times.
        /// </summary>
        static Dictionary<string, VirtualCurrencyRechargeTime> virtualCurrencyRechargeTime;

        /// <summary>
        ///     When the inventory was last updated.
        /// </summary>
        static DateTime lastUpdateTime = DateTime.MinValue;

        /// <summary>
        ///     Event invoked whenever the inventory is fetched.
        /// </summary>
        public static event Action OnInventoryUpdated = () => { };

        /// <summary>
        ///     True if local cache for inventory and virtual currency is not null.
        /// </summary>
        public static bool IsInitialized { get { return inventory != null && virtualCurrency != null; } }

        /// <summary>
        ///     True if API is currently fetching the player's inventory.
        /// </summary>
        public static bool IsFetching { get; private set; }

        /// <summary>
        ///     True if API is expired, not initialized, and not fetching.
        /// </summary>
        public static bool ShouldRefresh {
            get {
                if (IsFetching)
                    return false;
                if (!IsInitialized)
                    return true;
                if (IsExpired())
                    return true;
                return false;
            } set {
                lastUpdateTime = value ? DateTime.MinValue : DateTime.UtcNow;
            }
        }

        /// <summary>
        ///     Local System time for cache expiration checking.
        /// </summary>
        public static DateTime LastUpdateTime { get { return lastUpdateTime; } }

        /// <summary>
        ///     Gets a collection of ItemInstance of current player. Will fetch from PlayFab when cache is considered expired. Otherwise immediately invokes callback.
        /// </summary>
        /// <param name="onFetch">Callback for retrieval finish.</param>
        public static void GetInventory(Action<List<ItemInstance>> onFetch) {
            Action onFinish = () => onFetch.Invoke(inventory);

            if (IsFetching) {
                AwaitUpdate(onFinish);
            } else if (ShouldRefresh) {
                FetchInventory(onFinish);
            } else {
                onFinish.Invoke();
            }
        }

        /// <summary>
        ///     Fetch collection of ItemInstance of current player from PlayFab. Invokes callback after fetching.
        /// </summary>
        /// <param name="onSuccess">Callback for retrieval finish.</param>
        public static void FetchInventory(Action onSuccess) {
            if (IsFetching) {
                AwaitUpdate(onSuccess);
                return;
            }

            IsFetching = true;

            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                result => {
                    virtualCurrency = result.VirtualCurrency;
                    virtualCurrencyRechargeTime = result.VirtualCurrencyRechargeTimes;
                    inventory = result.Inventory;
            
                    lastUpdateTime = DateTime.UtcNow;
                    IsFetching = false;

                    OnInventoryUpdated.Invoke();
                    onSuccess.Invoke();
                }, error => { NetworkMethods.CheckNetworkError(error, () => { FetchInventory(onSuccess); }); }
            );
        }

        /// <summary>
        ///     Gets the inventory of the current player.
        ///     NOTE: this does not ensure that the player inventory is initialized or recently refreshed. It is recommended to use GetInventory instead, and pass an action to the callback parameter.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInstance> GetAllItems() {
            return inventory;
        }

        /// <summary>
        ///     From local cache, get inventory items of current player, filtered by ItemId.
        /// </summary>
        /// <param name="itemId">String of ItemId to filter for.</param>
        /// <returns>List of PlayFab ItemInstances.</returns>
        public static List<ItemInstance> GetItemsViaItemId(string itemId) {
            var items =
                from item in inventory
                where item.ItemId == itemId
                select item;
            return items.ToList();
        }

        /// <summary>
        ///     Get inventory items of current player, filtered by ItemId.
        ///     Will refresh inventory if local cache is expired.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="onFetch"></param>
        public static void GetItemsViaItemId(string itemId, Action<List<ItemInstance>> onFetch) {
            GetInventory(inventory => {
                onFetch.Invoke(GetItemsViaItemId(itemId));
            });
        }

        /// <summary>
        ///     Get virtual currencies of current player, filtered by currency code.
        /// </summary>
        /// <param name="currencyCode">Two-character string for PlayFab CurrencyCode to get.</param>
        /// <returns>Integer value for Currency amount.</returns>
        public static int GetVirtualCurrency(string currencyCode) {
            return virtualCurrency.ContainsKey(currencyCode) ? virtualCurrency[currencyCode] : 0;
        }

        /// <summary>
        ///     Get virtual currencies of current player, filtered by currency code.
        ///     Will refresh inventory if local cache is expired.
        /// </summary>
        /// <param name="currencyCode">Two-character string for PlayFab CurrencyCode to get.</param>
        /// <param name="onFetch">Callback for a successful operation.</param>
        public static void GetVirtualCurrency(string currencyCode, Action<int> onFetch) {
            GetInventory(inventory => {
                onFetch.Invoke(GetVirtualCurrency(currencyCode));
            });
        }

        /// <summary>
        ///     Get virtual currency recharge time of current player, filtered by currency code.
        /// </summary>
        /// <param name="currencyCode">Two-character string for PlayFab CurrencyCode to get.</param>
        /// <returns>Integer value for Currency recharge time.</returns>
        public static VirtualCurrencyRechargeTime GetVirtualCurrencyRechargeTime(string currencyCode) {
            return virtualCurrencyRechargeTime.ContainsKey(currencyCode) ? virtualCurrencyRechargeTime[currencyCode] : null;
        }

        /// <summary>
        ///     Get virtual currency recharge time of current player, filtered by currency code.
        /// </summary>
        /// <param name="currencyCode">Two-character string for PlayFab CurrencyCode to get.</param>
        /// <param name="onFetch">Callback for a successful operation.</param>
        public static void GetVirtualCurrencyRechargeTime(string currencyCode, Action<VirtualCurrencyRechargeTime> onFetch) {
            GetInventory(inventory => {
                onFetch.Invoke(GetVirtualCurrencyRechargeTime(currencyCode));
            });
        }

        public static IEnumerable<KeyValuePair<string, int>> GetAllVirtualCurrencies() {
            return virtualCurrency.ToList();
        }

        /// <summary>
        ///     Modifies the local value of the currency.
        ///     Does not in any way affect the actual server values.
        ///     Intended to be used for display purposes.
        /// </summary>
        /// <param name="currencyCode">String value of the currency code.</param>
        /// <param name="amount">Amount to add.</param>
        public static void ModifyVirtualCurrency(string currencyCode, int amount) {
            if (!virtualCurrency.ContainsKey(currencyCode))
                virtualCurrency.Add(currencyCode, amount);
            else
                virtualCurrency[currencyCode] += amount;

            OnInventoryUpdated.Invoke();
        }

        static void AwaitUpdate(Action awaitInventoryUpdate) {
            awaitInventoryUpdate += () => OnInventoryUpdated -= awaitInventoryUpdate;
            OnInventoryUpdated += awaitInventoryUpdate;
        }
        
        /// <summary>
        ///     Check if locally-cached player inventory is expired.
        /// </summary>
        /// <param name="cacheDuration">Floating-point value for amount of seconds exceeded for the cache to count as expired.</param>
        /// <returns>True if cache is expired.</returns>
        public static bool IsExpired(double cacheDuration = 60) {
            return (DateTime.UtcNow - lastUpdateTime).TotalSeconds >= cacheDuration;
        }
    }
}