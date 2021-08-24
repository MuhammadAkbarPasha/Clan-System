using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chamoji.Social {
    public static class PlayerDataAPI {

        /// <summary>
        ///     Table for PlayFab PlayerData, locally cached.
        /// </summary>
        static Dictionary<string, FetchData> data = new Dictionary<string, FetchData>();

        /// <summary>
        ///     Invoked whenever cached PlayFab Player Data for a specific key is updated.
        /// </summary>
        public static event Action<string> OnPlayerDataUpdated = key => { };

        /// <summary>
        ///     Fetch PlayFab PlayerData and cache it in locally-cached PlayerData table.
        /// </summary>
        /// <param name="key">Specific key to fetch from PlayFab PlayerData table.</param>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        public static void FetchData(string key, Action onSuccess) { FetchData(onSuccess, key); }

        /// <summary>
        ///     Fetch PlayFab PlayerData and cache it in locally-cached PlayerData table.
        /// </summary>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        /// <param name="keys">List of keys to fetch from PlayFab PlayerData table.</param>
        public static void FetchData(Action onSuccess, params string[] keys) { FetchData(keys, onSuccess); }
        /// <summary>
        /// Get specific user data. For debug purposes only!
        /// </summary>
        /// <param name="playFabId"></param>
        /// <param name=""></param>
        public static void FetchSpecificUserData(string playFabId, string key, Action onSuccess) { FetchSpecificUserData(playFabId, onSuccess, key); }
        public static void FetchSpecificUserData(string playFabId, Action onSuccess, params string[] keys) { GetSpecificUserData(playFabId, keys, onSuccess); }
        public static void GetSpecificUserData(string playFabId, IEnumerable<string> keys, Action onSuccess)
        {
            PlayFabClientAPI.GetUserData(
                new GetUserDataRequest()
                {
                    PlayFabId = playFabId,
                    Keys = keys.ToList()
                }, result => {
                    foreach (var key in keys)
                    {
                        var hasKey = result.Data.ContainsKey(key);
                        var fetchData = new FetchData()
                        {
                            lastUpdated = DateTime.UtcNow,
                            value = hasKey ? result.Data[key].Value : string.Empty
                        };
                        if (!data.ContainsKey(key))
                            data.Add(key, fetchData);
                        else
                            data[key] = fetchData;

                        onSuccess.Invoke();
                        OnPlayerDataUpdated.Invoke(key);
                    }
                },
                 errorCallback => { NetworkMethods.CheckNetworkError(errorCallback, () => { FetchData(keys, onSuccess); }); }
            );
        }

        /// <summary>
        ///     Fetch PlayFab PlayerData and cache it in locally-cached PlayerData table.
        /// </summary>
        /// <param name="keys">List of keys to fetch from PlayFab PlayerData table.</param>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        public static void FetchData(IEnumerable<string> keys, Action onSuccess) {
            PlayFabClientAPI.GetUserData(
                new GetUserDataRequest() {
                    PlayFabId = "72190E9A23C80441",
                    Keys = keys.ToList()
                }, result => {
                    foreach (var key in keys) {
                        var hasKey = result.Data.ContainsKey(key);
                        var fetchData = new FetchData() {
                            lastUpdated = DateTime.UtcNow,
                            value = hasKey ? result.Data[key].Value : string.Empty
                        };
                        if (!data.ContainsKey(key))
                            data.Add(key, fetchData);
                        else
                            data[key] = fetchData;

                        onSuccess.Invoke();
                        OnPlayerDataUpdated.Invoke(key);
                    }
                },
                 errorCallback => { NetworkMethods.CheckNetworkError(errorCallback, () => { FetchData(keys,onSuccess); }); }
            );
        }

        /// <summary>
        ///     Overwrite values in PlayFab PlayerData with values from PlayerData table in local cache.
        /// </summary>
        /// <param name="keys">List of keys for values to overwrite.</param>
        /// <param name="onSuccess">Callback for a successful operation.</param>
        public static void PostData(IEnumerable<string> keys, Action onSuccess) { PostData(onSuccess, keys.ToArray()); }

        /// <summary>
        ///     Overwrite values in PlayFab PlayerData with values from PlayerData table in local cache.
        /// </summary>
        /// <param name="onSuccess">List of keys for values to overwrite.</param>
        /// <param name="keys">Callback for a successful operation.</param>
        public static void PostData(Action onSuccess, params string[] keys) {
            var pairs =
                from pair in data
                where data.ContainsKey(pair.Key)
                select pair;
            var output = pairs.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.value
            );

            PlayFabClientAPI.UpdateUserData(
                new UpdateUserDataRequest() {
                    Data = output
                }, result => onSuccess.Invoke(),
                 errorCallback => { NetworkMethods.CheckNetworkError(errorCallback, () => { PostData(onSuccess, keys); }); }
            );
        }

        public static void PostSpecificUserData(string playfabId, Action onSuccess, params string[] keys)
        {
            var pairs =
                from pair in data
                where data.ContainsKey(pair.Key)
                select pair;
            var output = pairs.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.value
            );

            PlayFabClientAPI.UpdateUserData(
                new UpdateUserDataRequest()
                {
                    Data = output
                }, result => onSuccess.Invoke(),
                 errorCallback => { NetworkMethods.CheckNetworkError(errorCallback, () => { PostSpecificUserData(playfabId, onSuccess, keys); }); }
            );
        }

        /// <summary>
        ///     Update value of a row in PlayerData table in local cache with the given object, deserialized into JSON.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="key">Key for PlayFab.</param>
        /// <param name="value">Object to be saved, deserialized into JSON.</param>
        /// <param name="overwrite">If true, already-existing local cache value should be overwritten.</param>
        /// <param name="cloudSave">If true, also overwrite PlayFab PlayerData value of specific key.</param>
        public static void UpdateData<T>(string key, T value, bool overwrite = false, bool cloudSave = false) {
            if (!data.ContainsKey(key)) {
                data.Add(key, new FetchData() {
                    lastUpdated = DateTime.UtcNow,
                    value = JsonConvert.SerializeObject(value)
                });
            } else if (overwrite) {
                data[key].lastUpdated = DateTime.UtcNow;
                data[key].value = JsonConvert.SerializeObject(value);
            } else {
                var message = string.Format("Data is already bound to {0}", key);
                throw new OperationCanceledException(message);
            }

            OnPlayerDataUpdated.Invoke(key);

            if (cloudSave)
                PostData(() => { }, key);
        }

        /// <summary>
        ///     Remove row from PlayerData table in local cache.
        /// </summary>
        /// <param name="key">Key of row to remove.</param>
        public static void RemoveData(string key) {
            if (!data.ContainsKey(key))
                return;

            data.Remove(key);
            OnPlayerDataUpdated.Invoke(key);
        }

        /// <summary>
        ///     Remove all rows from PlayerData table in local cache.
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
        public static void GetData<T>(string key, Action<T> onFetch, string emptyJson = "{}") {
            if (IsDataExpired(key))
                FetchData( key, () => onFetch.Invoke(JsonConvert.DeserializeObject<T>(data[key]?.value ?? emptyJson)) );
            else
                onFetch.Invoke(JsonConvert.DeserializeObject<T>(data[key]?.value ?? emptyJson));
        }

        /// <summary>
        /// Fetching specific user data. Used for debugging and testing purposes only!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playfabId"></param>
        /// <param name="key"></param>
        /// <param name="onFetch"></param>
        /// <param name="emptyJson"></param>
        public static void GetSpecificData<T>(string playfabId, string key, Action<T> onFetch, string emptyJson = "{}")
        {
            if (IsDataExpired(key))
               FetchSpecificUserData(playfabId, key, () => onFetch.Invoke(JsonConvert.DeserializeObject<T>(data[key]?.value ?? emptyJson)));
            else
                onFetch.Invoke(JsonConvert.DeserializeObject<T>(data[key]?.value ?? emptyJson));
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