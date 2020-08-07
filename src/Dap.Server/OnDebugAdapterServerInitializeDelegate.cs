using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public delegate Task OnDebugAdapterServerInitializeDelegate(IDebugAdapterServer server, InitializeRequestArguments request, CancellationToken cancellationToken);
}