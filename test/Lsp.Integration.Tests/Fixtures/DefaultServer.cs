using OmniSharp.Extensions.LanguageServer.Server;

namespace Lsp.Integration.Tests.Fixtures
{
    public sealed class DefaultServer : IConfigureLanguageServerOptions
    {
        public void Configure(LanguageServerOptions options)
        {
        }
    }
}
