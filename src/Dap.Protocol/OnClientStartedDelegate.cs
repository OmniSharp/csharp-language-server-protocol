using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public delegate Task OnClientStartedDelegate(IDebugAdapterClient client, InitializeResponse result, CancellationToken cancellationToken);
}