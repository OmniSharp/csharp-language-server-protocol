using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="IDebugAdapterClient" /> after the connection has been established.
    /// </summary>
    public interface IOnDebugAdapterClientStarted : IEventingHandler
    {
        Task OnStarted(IDebugAdapterClient client, CancellationToken cancellationToken);
    }
}
