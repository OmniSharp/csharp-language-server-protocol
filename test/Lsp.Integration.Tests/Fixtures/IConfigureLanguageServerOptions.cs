using OmniSharp.Extensions.LanguageServer.Server;

namespace Lsp.Tests.Integration.Fixtures
{
    public interface IConfigureLanguageServerOptions
    {
        void Configure(LanguageServerOptions options);
    }
}