using System;
using UnityEngine;

namespace BeatThat.NetworkStatus
{
    public struct NetworkStatusData 
    {
        public bool hasNetworkError;
        public NetworkReachability networkReachability;
        public DateTime lastNetworkSuccess;
        public DateTime lastNetworkError;

        public bool IsNetworkReachableWithoutError()
        {
            return IsNetworkReachable() && !this.hasNetworkError;
        }

        public bool IsNetworkReachable()
        {
            return this.networkReachability != NetworkReachability.NotReachable;
        }

        public bool HasLastNetworkSuccess()
        {
            return this.lastNetworkSuccess.Ticks > default(DateTime).Ticks;
        }

        public bool HasLastNetworkError()
        {
            return this.lastNetworkError.Ticks > default(DateTime).Ticks;
        }
    }
}

