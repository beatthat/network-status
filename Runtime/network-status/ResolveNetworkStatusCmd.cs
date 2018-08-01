using BeatThat.Commands;
using BeatThat.StateStores;

namespace BeatThat.NetworkStatus
{
    [RegisterCommand]
    public class ResolveNetworkStatusCmd : ResolveStateCmd<NetworkStatusData>
    {
       
    }
}
