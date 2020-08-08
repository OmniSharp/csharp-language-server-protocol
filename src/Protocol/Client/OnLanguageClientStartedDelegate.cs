using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public delegate Task OnLanguageClientStartedDelegate(ILanguageClient client, InitializeResult result, CancellationToken cancellationToken);
}
