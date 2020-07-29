using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public delegate Task InitializedDelegate(IDebugAdapterServer server, InitializeRequestArguments request, InitializeResponse response, CancellationToken cancellationToken);
}