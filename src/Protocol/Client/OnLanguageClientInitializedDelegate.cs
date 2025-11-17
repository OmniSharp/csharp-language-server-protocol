using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="InitializeParams" /> and <see cref="InitializeResult" /> before it is processed by the client.
    /// </summary>
    public delegate Task OnLanguageClientInitializedDelegate(ILanguageClient client, InitializeParams request, InitializeResult response, CancellationToken cancellationToken);
}
