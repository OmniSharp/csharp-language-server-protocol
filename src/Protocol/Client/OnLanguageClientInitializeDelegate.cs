using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams" /> before it is sent to the server
    /// </summary>
    public delegate Task OnLanguageClientInitializeDelegate(ILanguageClient client, InitializeParams request, CancellationToken cancellationToken);
}
