using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams" /> before it is sent to the server
    /// </summary>
    public interface IOnLanguageClientInitialize : IEventingHandler
    {
        Task OnInitialize(ILanguageClient client, InitializeParams result, CancellationToken cancellationToken);
    }
}
