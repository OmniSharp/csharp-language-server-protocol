using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    public delegate Task OnDebugAdapterServerStartedDelegate(IDebugAdapterServer server, InitializeResponse result, CancellationToken cancellationToken);
}