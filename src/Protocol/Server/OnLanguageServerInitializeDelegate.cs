using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public delegate Task OnLanguageServerInitializeDelegate(ILanguageServer server, InitializeParams request, CancellationToken cancellationToken);
}
