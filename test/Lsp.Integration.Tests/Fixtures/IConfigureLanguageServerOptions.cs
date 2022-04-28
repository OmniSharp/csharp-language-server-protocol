using OmniSharp.Extensions.LanguageServer.Server;

namespace Lsp.Integration.Tests.Fixtures
{
    public interface IConfigureLanguageServerOptions
    {
        void Configure(LanguageServerOptions options);
    }
}
