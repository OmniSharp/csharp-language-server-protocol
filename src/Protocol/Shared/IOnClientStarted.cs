using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public interface IOnClientStarted
    {
        Task OnStarted(ILanguageClient server, InitializeResult result, CancellationToken cancellationToken);
    }
}
