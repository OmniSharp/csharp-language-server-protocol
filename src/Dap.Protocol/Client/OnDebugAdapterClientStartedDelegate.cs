using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    public delegate Task OnDebugAdapterClientStartedDelegate(IDebugAdapterClient client, InitializeResponse result, CancellationToken cancellationToken);
}
