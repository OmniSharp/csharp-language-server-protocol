using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="IDebugAdapterServer" /> after the connection has been established.
    /// </summary>
    public interface IOnDebugAdapterServerStarted : IEventingHandler
    {
        Task OnStarted(IDebugAdapterServer server, CancellationToken cancellationToken);
    }
}
