using OmniSharp.Extensions.LanguageServer.Client;

namespace Lsp.Tests.Integration.Fixtures
{
    public interface IConfigureLanguageClientOptions
    {
        void Configure(LanguageClientOptions options);
    }
}