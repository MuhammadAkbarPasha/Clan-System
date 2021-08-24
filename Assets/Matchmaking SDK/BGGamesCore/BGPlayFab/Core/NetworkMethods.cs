using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chamoji.Social {
    public static class NetworkMethods {

        public static Action<PlayFabError> OnNetworkError;
        public static Action<PlayFabError,RetryMethod> OnNetworkErrorCallback;
        public static Action<PlayFabError> OnMaintenanceState = error => Debug.LogError(error);
        public delegate void RetryMethod();
   
       /// <summary>
        ///     Force event for network error.
        /// </summary>
        public static void NotifyNetworkError() {
            OnNetworkError(new PlayFabError() {
                Error = PlayFabErrorCode.ServiceUnavailable,
                ErrorMessage = "Network error forced."
            });
        }

        /// <summary>
        ///     Check network connectivity.
        /// </summary>
        /// <returns>True if connectivity exists. Otherwise returns false.</returns>
        public static bool CheckNetworkError() {
            bool networkUnreachable = Application.internetReachability == NetworkReachability.NotReachable;
            if (networkUnreachable)
                NotifyNetworkError();
            return networkUnreachable;
        }

        public static void CheckNetworkError(PlayFabError error)
        {
            CheckNetworkError(error, null);
        }
        /// <summary>
        ///     Check PlayFab network error type.
        ///     Run respective events depending on PLayFab error code.
        /// </summary>
        /// <param name="error"></param>
        public static void CheckNetworkError(PlayFabError error, RetryMethod method = null)
        {
            if (error.Error == PlayFabErrorCode.ServiceUnavailable)
                OnNetworkErrorCallback.Invoke(error,method);
            else if (error.Error == PlayFabErrorCode.ConnectionError)
                OnNetworkErrorCallback.Invoke(error,method);
            else if (error.Error == PlayFabErrorCode.NotAuthorizedByTitle)
                OnMaintenanceState.Invoke(error);
            else
                Debug.LogError(error);
        }



    }
}