using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface IOnLanguageClientStarted
    {
        Task OnStarted(ILanguageClient server, InitializeResult result, CancellationToken cancellationToken);
    }
}
