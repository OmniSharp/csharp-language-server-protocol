using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="ILanguageClient" /> after the connection has been established.
    /// </summary>
    public interface IOnLanguageClientStarted : IEventingHandler
    {
        Task OnStarted(ILanguageClient client, CancellationToken cancellationToken);
    }
}
