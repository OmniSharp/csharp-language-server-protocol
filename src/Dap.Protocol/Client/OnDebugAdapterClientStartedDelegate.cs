using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="IDebugAdapterClient" /> after the connection has been established.
    /// </summary>
    public delegate Task OnDebugAdapterClientStartedDelegate(IDebugAdapterClient client, CancellationToken cancellationToken);
}
