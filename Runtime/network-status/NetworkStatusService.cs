using BeatThat.Bindings;
using BeatThat.DependencyInjection;
using BeatThat.Service;
using BeatThat.StateStores;
using UnityEngine;

namespace BeatThat.NetworkStatus
{
    [RegisterService]
    public class NetworkStatusService : BindingService
    {
        private const float POLLING_INTERVAL_SECS = 5f;

        [Inject] HasState<NetworkStatusData> networkStatus;

        protected override void BindAll()
        {
            Bind(State<NetworkStatusData>.UPDATED, this.OnNetworkStatusUpdated);
            OnNetworkStatusUpdated();
        }

        private void OnNetworkStatusUpdated()
        {
            var state = this.networkStatus.stateData;

            if(!state.hasNetworkError) {
                StopAllCoroutines();
                return;
            }

            if(state.networkReachability == NetworkReachability.NotReachable) {
                return;
            }

            PollForNetwork();
        }

        private void PollForNetwork()
        {
            this.pollTimer = POLLING_INTERVAL_SECS;
            this.enabled = true;
        }


        private float pollTimer;

        void Update()
        {
            var state = this.networkStatus.stateData;

            if(!state.hasNetworkError) {
                this.enabled = false;
                this.pollTimer = 0f;
                return;
            }

            if(state.networkReachability == NetworkReachability.NotReachable) {
                this.enabled = false;
                this.pollTimer = 0f;
                return;
            }

            this.pollTimer -= Time.unscaledDeltaTime;

            if(this.pollTimer <= 0) {
                State<NetworkStatusData>.ResolveRequested(new ResolveRequestDTO
                {
                    forceUpdate = true
                });
                this.pollTimer = POLLING_INTERVAL_SECS;
            }
        }
    }
}
