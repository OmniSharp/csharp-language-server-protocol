using OmniSharp.Extensions.LanguageServer.Client;

namespace Lsp.Integration.Tests.Fixtures
{
    public interface IConfigureLanguageClientOptions
    {
        void Configure(LanguageClientOptions options);
    }
}
