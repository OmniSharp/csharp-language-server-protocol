using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="ILanguageServer" /> after the connection has been established.
    /// </summary>
    public interface IOnLanguageServerStarted : IEventingHandler
    {
        Task OnStarted(ILanguageServer server, CancellationToken cancellationToken);
    }
}
