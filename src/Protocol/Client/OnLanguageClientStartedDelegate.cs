namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="ILanguageClient" /> after the connection has been established.
    /// </summary>
    public delegate Task OnLanguageClientStartedDelegate(ILanguageClient client, CancellationToken cancellationToken);
}
