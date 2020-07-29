using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public delegate Task InitializeDelegate(IDebugAdapterServer server, InitializeRequestArguments request, CancellationToken cancellationToken);
}