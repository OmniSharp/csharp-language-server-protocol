using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="ILanguageServer" /> after the connection has been established.
    /// </summary>
    public delegate Task OnLanguageServerStartedDelegate(ILanguageServer server, CancellationToken cancellationToken);
}
