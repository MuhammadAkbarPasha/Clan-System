using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chamoji.Social {
    public static class TitleDataAPI {

        /// <summary>
        ///     Table for TitleData, locally cached.
        /// </summary>
        static Dictionary<string, FetchData> data = new Dictionary<string, FetchData>();

        /// <summary>
        ///     Invoked whenever TitleData table in local cache is updated.
        /// </summary>
        public static event Action OnTitleDataUpdated = () => { };

        /// <summary>
        ///     Fetch row from PlayFab TitleData table, and cache in local TitleData table.
        /// </summary>
        /// <param name="key">Key of row to fetch.</param>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        public static void FetchData(string key, Action onSuccess) { FetchData(onSuccess, key); }

        /// <summary>
        ///     Fetch row from PlayFab TitleData table, and cache in local TitleData table.
        /// </summary>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        /// <param name="keys">Keys of rows to fetch.</param>
        public static void FetchData(Action onSuccess, params string[] keys) { FetchData(keys, onSuccess); }

        /// <summary>
        ///     Fetch row from PlayFab TitleData table, and cache in local TitleData table.
        /// </summary>
        /// <param name="keys">Keys of rows to fetch.</param>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        public static void FetchData(IEnumerable<string> keys, Action onSuccess) {
            PlayFabClientAPI.GetTitleData(
                new GetTitleDataRequest() {
                    Keys = keys.ToList()
                }, result => {
                    foreach (var key in keys) {
                        var hasKey = result.Data.ContainsKey(key);
                        var fetchData = new FetchData() {
                            lastUpdated = DateTime.UtcNow,
                            value = hasKey ? result.Data[key] : string.Empty
                        };

                        if (!data.ContainsKey(key))
                            data.Add(key, fetchData);
                        else
                            data[key] = fetchData;

                        onSuccess.Invoke();
                        OnTitleDataUpdated.Invoke();
                    }
                },
                error => { NetworkMethods.CheckNetworkError(error, () => { FetchData(keys,onSuccess); }); }
            );
        }

        /// <summary>
        ///     Remove row from TitleData table in local cache.
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveData(string key) {
            if (data.ContainsKey(key))
                data.Remove(key);
        }

        /// <summary>
        ///     Remove row all rows from TitleData table in local cache.
        /// </summary>
        public static void ClearData() {
            data.Clear();
        }

        /// <summary>
        ///     Get data mapped to a given key.
        ///     Will attempt to fetch if there is no data mapped to the given key,
        ///     or if the data mapped to the key is already expired.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize to.</typeparam>
        /// <param name="key">String identifier.</param>
        /// <param name="onFetch">Callback for finishing the fetch.</param>
        public static void GetData<T>(string key, Action<T> onFetch) {
            if (IsDataExpired(key))
                FetchData( key, () => onFetch.Invoke(JsonConvert.DeserializeObject<T>(data[key].value)) );
            else
                onFetch.Invoke(JsonConvert.DeserializeObject<T>(data[key].value));
        }

        /// <summary>
        ///     In order: checks if there is data mapped to the given key,
        ///     checks if the data mapped to the key is not expired,
        ///     and checks if the data mapped to the key is not null or empty.
        /// </summary>
        /// <param name="key">String identifier.</param>
        /// <returns>True if all conditions are met. Otherwise returns false.</returns>
        public static bool IsDataValid(string key) {
            if (IsDataExpired(key))
                return false;
            if (string.IsNullOrEmpty(data[key].value))
                return false;
            return true;
        }

        /// <summary>
        ///     Checks if there has been an attempt to fetch data for the given key.
        /// </summary>
        /// <param name="key">String identifier.</param>
        /// <returns>True if player has attempted to fetch data for the given key. Otherwise returns false.</returns>
        public static bool IsDataInitialized(string key) {
            return data.ContainsKey(key);
        }

        /// <summary>
        ///     Checks if data is expired.
        /// </summary>
        /// <param name="key">String identifier.</param>
        /// <param name="cacheDuration">Double value for amount of seconds before the data is considered expired. Set to 60 if unspecified.</param>
        /// <returns>Returns true if data has been in the cache for longer than the cache duration specifies, or if there is no data mapped to the specified key. Otherwise returns false.</returns>
        public static bool IsDataExpired(string key, double cacheDuration = 60) {
            if (!IsDataInitialized(key))
                return true;
            return (DateTime.UtcNow - data[key].lastUpdated).TotalSeconds >= cacheDuration;
        }

    }
}