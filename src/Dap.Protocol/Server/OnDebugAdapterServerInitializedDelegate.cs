using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeRequestArguments" /> and <see cref="InitializeResponse" /> after it is processed by the server but before it is sent to the client
    /// </summary>
    public delegate Task OnDebugAdapterServerInitializedDelegate(
        IDebugAdapterServer server, InitializeRequestArguments request, InitializeResponse response, CancellationToken cancellationToken
    );
}
