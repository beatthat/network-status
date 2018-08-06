using System;
using BeatThat.NetworkNotifications;
using BeatThat.Service;
using BeatThat.StateStores;
using UnityEngine;
using UnityEngine.Networking;

namespace BeatThat.NetworkStatus
{
    [RegisterService(
        proxyInterfaces: new Type[] {
        typeof(StateStore<NetworkStatusData>),
        typeof(HasState<NetworkStatusData>)
    }
    )]
    public class NetworkStatusStore : StateStore<NetworkStatusData>
    {
        protected override void BindStore()
        {
            Bind<UnityWebRequest>(NetworkNotification.WEB_REQUEST_RECEIVED_RESPONSE, this.OnNetworkResponse);
            Bind<UnityWebRequest>(NetworkNotification.WEB_REQUEST_NETWORK_ERROR, this.OnNetworkError);
        }

        void Update()
        {
            if(!this.isBound) {
                return;
            }

            var data = this.stateData;

            var nr = Application.internetReachability;
            if (nr == data.networkReachability)
            {
                return;
            }

            data.networkReachability = nr;
            UpdateData(ref data);
        }


        private void OnNetworkResponse(UnityWebRequest www)
        {
            if(www.uri.IsFile) {
                return;
            }


            var s = this.state;

            var d = s.data;
            var statusChanged = d.hasNetworkError;

            d.hasNetworkError = false;
            d.lastNetworkSuccess = DateTime.Now;
            s.data = d;

            UpdateState(ref s, statusChanged);
        }

        private void OnNetworkError(UnityWebRequest www)
        {
            var s = this.state;

            var d = s.data;
            var statusChanged = d.hasNetworkError == false;
            d.hasNetworkError = true;
            d.lastNetworkError = DateTime.Now;
            s.data = d;

            UpdateState(ref s, statusChanged);
        }
    }
}

