using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams" /> and <see cref="InitializeResult" /> after it is processed by the server but before it is sent to the client
    /// </summary>
    public interface IOnLanguageServerInitialized : IEventingHandler
    {
        Task OnInitialized(ILanguageServer server, InitializeParams request, InitializeResult result, CancellationToken cancellationToken);
    }
}
