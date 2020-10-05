using OmniSharp.Extensions.LanguageServer.Server;

namespace Lsp.Tests.Integration.Fixtures
{
    public sealed class DefaultServer : IConfigureLanguageServerOptions
    {
        public void Configure(LanguageServerOptions options)
        {
        }
    }
}
