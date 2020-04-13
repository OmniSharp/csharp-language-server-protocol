using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public delegate Task InitializeDelegate(ILanguageServer server, InitializeParams request, CancellationToken cancellationToken);
    public delegate Task StartedDelegate(ILanguageServer server, InitializeResult result, CancellationToken cancellationToken);

    public interface IOnStarted
    {
        Task OnStarted(ILanguageServer server, InitializeResult result, CancellationToken cancellationToken);
    }
}
