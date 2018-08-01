using System.Collections;
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

            StopAllCoroutines();
            StartCoroutine(PollForNetwork());
        }

        private IEnumerator PollForNetwork()
        {
            // todo, interval should increase over time etc and be configurable
            while(true) {
                yield return new WaitForSeconds(5);
                if(!this.networkStatus.stateData.hasNetworkError) {
                    break;
                }

                State<NetworkStatusData>.ResolveRequested(new ResolveRequestDTO {
                    forceUpdate = true
                });
            }
        }
    }
}
