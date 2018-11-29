using System;
using BeatThat.DependencyInjection;
using BeatThat.Requests;
using BeatThat.Service;
using BeatThat.StateStores;
using UnityEngine;

#if NET_4_6
using System.Threading.Tasks;
#endif

namespace BeatThat.NetworkStatus
{
    [RegisterService(
        proxyInterfaces: new Type[] { typeof(StateResolver<NetworkStatusData>) },
        priority: int.MaxValue // so will be overridden by anything with a lower priority
    )]
    public class ResolveNetworkStatusByTestURL : DefaultStateResolver<NetworkStatusData>
    {
        virtual protected string NextTestUrl()
        {
            return "https://www.google.com";
        }

#if NET_4_6
        public async Task<ResolveResultDTO<NetworkStatusData>> ResolveAsync(
            Action<Request<ResolveResultDTO<NetworkStatusData>>> callback
        )
        {
            var nstatus = new NetworkStatusData
            {
                networkReachability = Application.internetReachability
            };

            if(nstatus.networkReachability == NetworkReachability.NotReachable) {
                return ResolveResultDTO<NetworkStatusData>.ResolveSucceeded(nstatus);
            }

            var req = new WebRequest(NextTestUrl());

            await req.ExecuteAsyncTask();

            var state = new NetworkStatusData();

            state.hasNetworkError = req.www.isNetworkError;

            if(state.hasNetworkError) {
                state.lastNetworkError = DateTime.Now;
            }
            else {
                state.lastNetworkSuccess = DateTime.Now;
            }

            return ResolveResultDTO<NetworkStatusData>.ResolveSucceeded(nstatus);
        }
#else
        override public Request<ResolveResultDTO<NetworkStatusData>> Resolve(
            Action<Request<ResolveResultDTO<NetworkStatusData>>> callback = null)
        {
            var nstatus = new NetworkStatusData
            {
                networkReachability = Application.internetReachability
            };

            if (nstatus.networkReachability == NetworkReachability.NotReachable)
            {
                var res = new LocalRequest<ResolveResultDTO<NetworkStatusData>>(
                    ResolveResultDTO<NetworkStatusData>.ResolveSucceeded(nstatus)
                );
                res.Execute(callback);
                return res;
            }

            var promise = new Promise<ResolveResultDTO<NetworkStatusData>>((resolve, reject) =>
            {
                new WebRequest(NextTestUrl()).Execute(req =>
                {
                    var state = new NetworkStatusData();

                    state.hasNetworkError = (req as WebRequest).www.isNetworkError;

                    if (state.hasNetworkError)
                    {
                        state.lastNetworkError = DateTime.Now;
                    }
                    else
                    {
                        state.lastNetworkSuccess = DateTime.Now;
                    }

                    resolve(
                        ResolveResultDTO<NetworkStatusData>.ResolveSucceeded(nstatus)
                    );
                });
            });

            promise.Execute(callback);

            return promise;   
        }
#endif

        [Inject] HasState<NetworkStatusData> networkStatus;
    }
}
