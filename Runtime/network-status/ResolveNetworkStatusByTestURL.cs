using System;
using BeatThat.Bindings;
using BeatThat.DependencyInjection;
using BeatThat.Requests;
using BeatThat.Service;
using BeatThat.StateStores;
using UnityEngine;

namespace BeatThat.NetworkStatus
{
    [RegisterService(
        proxyInterfaces: new Type[] { typeof(StateResolver<NetworkStatusData>) },
        priority: int.MaxValue // so will be overridden by anything with a lower priority
    )]
    public class ResolveNetworkStatusByTestURL : BindingService, StateResolver<NetworkStatusData>
    {
        virtual protected string NextTestUrl()
        {
            return "https://www.google.com";
        }

        public Request<ResolveResponseDTO<NetworkStatusData>> Resolve(Action<Request<ResolveResponseDTO<NetworkStatusData>>> callback)
        {
            var promise = new Promise<ResolveResponseDTO<NetworkStatusData>>((resolve, reject) =>
            {
                new WebRequest(NextTestUrl()).Execute(result =>
                {
                    var state = this.networkStatus.stateData;

                    state.hasNetworkError = (result as WebRequest).www.isNetworkError;

                    if(state.hasNetworkError) {
                        state.lastNetworkError = DateTime.Now;
                    }
                    else {
                        state.lastNetworkSuccess = DateTime.Now;
                    }

                    resolve(new ResolveResponseDTO<NetworkStatusData>
                    {
                        status = Constants.STATUS_OK,
                        data = state
                    });
                });
            });
            promise.Execute(callback);
            return promise;
        }

        [Inject] HasState<NetworkStatusData> networkStatus;
    }
}
