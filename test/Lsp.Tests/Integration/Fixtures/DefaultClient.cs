using OmniSharp.Extensions.LanguageServer.Client;

namespace Lsp.Tests.Integration.Fixtures
{
    public sealed class DefaultClient : IConfigureLanguageClientOptions
    {
        public void Configure(LanguageClientOptions options)
        {
        }
    }
}
