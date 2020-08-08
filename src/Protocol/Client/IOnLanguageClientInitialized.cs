using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams"/> and <see cref="InitializeResult"/> before it is processed by the client.
    /// </summary>
    public interface IOnLanguageClientInitialized : IEventingHandler
    {
        Task OnInitialized(ILanguageClient client, InitializeParams request, InitializeResult result, CancellationToken cancellationToken);
    }
}
