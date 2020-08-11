using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="IDebugAdapterServer" /> after the connection has been established.
    /// </summary>
    public delegate Task OnDebugAdapterServerStartedDelegate(IDebugAdapterServer server, CancellationToken cancellationToken);
}
