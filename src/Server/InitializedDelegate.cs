using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public delegate Task InitializedDelegate(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken cancellationToken);
}
