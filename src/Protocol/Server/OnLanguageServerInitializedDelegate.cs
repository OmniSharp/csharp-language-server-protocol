using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public delegate Task OnLanguageServerInitializedDelegate(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken cancellationToken);
}
