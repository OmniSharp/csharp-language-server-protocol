using OmniSharp.Extensions.LanguageServer.Client;

namespace Lsp.Tests.Integration.Fixtures
{
    public sealed class DefaultClient : IConfigureLanguageClientOptions
    {
        public DefaultClient()
        {
        }

        public void Configure(LanguageClientOptions options)
        {
        }
    }
}