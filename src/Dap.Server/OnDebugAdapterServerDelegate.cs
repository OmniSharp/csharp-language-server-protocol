using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public delegate Task OnDebugAdapterServerDelegate(IDebugAdapterServer server, InitializeRequestArguments request, InitializeResponse response, CancellationToken cancellationToken);
}