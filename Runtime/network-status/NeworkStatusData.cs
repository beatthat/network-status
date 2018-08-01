using System;

namespace BeatThat.NetworkStatus
{
    public struct NetworkStatusData 
    {
        public bool hasNetworkError;
        public DateTime lastNetworkSuccess;
        public DateTime lastNetworkError;

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

