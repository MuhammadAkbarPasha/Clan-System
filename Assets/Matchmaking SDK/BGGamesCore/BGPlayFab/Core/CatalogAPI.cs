using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chamoji.Social {
    public class CatalogAPI : MonoBehaviour {

        static List<CatalogItem> catalog;
        static DateTime lastUpdateTime = DateTime.MinValue;

        /// <summary>
        ///     Event invoked whenever the inventory is fetched.
        /// </summary>
        public static event Action OnCatalogUpdated = () => { };

        public static bool IsInitialized { get { return catalog != null; } }
        public static bool IsFetching { get; private set; }
        public static bool ShouldRefresh {
            get {
                if (IsFetching)
                    return false;
                if (!IsInitialized)
                    return true;
                if (IsExpired())
                    return true;
                return false;
            }
        }

        /// <summary>
        ///     Local System time for cache expiration checking.
        /// </summary>
        public static DateTime LastUpdateTime { get { return lastUpdateTime; } }

        /// <summary>
        ///     Gets a collection of CatalogItem. Will fetch from PlayFab when cache is considerd expired. Otherwise immediately invokes callback.
        /// </summary>
        /// <param name="onFetch">Callback for retrieval finish.</param>
        public static void GetCatalog(Action<List<CatalogItem>> onFetch) {
            Action onFinish = () => onFetch.Invoke(catalog);

            if (IsFetching) {
                AwaitUpdate(onFinish);
            } else if (ShouldRefresh) {
                FetchCatalog(onFinish);
            } else {
                onFinish.Invoke();
            }
        }

        /// <summary>
        ///     Fetch collection of CatalogItem from PlayFab. Invokes callback after fetching.
        /// </summary>
        /// <param name="onSuccess">Callback for retrieval finish.</param>
        public static void FetchCatalog(Action onSuccess) {
            if (IsFetching) {
                AwaitUpdate(onSuccess);
                return;
            }

            IsFetching = true;

            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest(),
                result => {
                    catalog = result.Catalog;
                    lastUpdateTime = DateTime.UtcNow;
                    IsFetching = false;
                    onSuccess.Invoke();
                    OnCatalogUpdated.Invoke();

                }, error => { NetworkMethods.CheckNetworkError(error, () => { FetchCatalog(onSuccess); }); }
            );
        }

        public static CatalogItem GetCatalogItem(string itemId) {
            return catalog.Find(entry => entry.ItemId == itemId);
        }

        public static void GetCatalogItem(string itemId, Action<CatalogItem> onFetch) {
            Action onFinish = () => onFetch.Invoke(GetCatalogItem(itemId));

            if (ShouldRefresh) {
                FetchCatalog(onFinish);
            } else if (IsFetching) {
                AwaitUpdate(onFinish);
            } else {
                onFinish.Invoke();
            }
        }

        public static List<CatalogItem> GetAllCatalogItems() {
            return catalog;
        }

        static void AwaitUpdate(Action awaitCatalogUpdate) {
            awaitCatalogUpdate += () => OnCatalogUpdated -= awaitCatalogUpdate;
            OnCatalogUpdated += awaitCatalogUpdate;
        }

        public static bool IsExpired(double cacheDuration = 300) {
            return (DateTime.UtcNow - lastUpdateTime).Seconds >= cacheDuration;
        }

    }
}