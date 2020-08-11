using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams" /> and <see cref="InitializeResult" /> after it is processed by the server but before it is sent to the client
    /// </summary>
    public delegate Task OnLanguageServerInitializedDelegate(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken cancellationToken);
}
