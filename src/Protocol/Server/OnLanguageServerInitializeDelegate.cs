using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams" /> before it is processed by the server
    /// </summary>
    public delegate Task OnLanguageServerInitializeDelegate(ILanguageServer server, InitializeParams request, CancellationToken cancellationToken);
}
