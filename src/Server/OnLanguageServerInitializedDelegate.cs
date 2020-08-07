using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public delegate Task OnLanguageServerInitializedDelegate(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken cancellationToken);
}
