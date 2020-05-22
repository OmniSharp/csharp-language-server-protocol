using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public delegate Task OnServerStartedDelegate(ILanguageServer server, InitializeResult result, CancellationToken cancellationToken);
}
