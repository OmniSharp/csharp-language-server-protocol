using OmniSharp.Extensions.JsonRpc.Testing;

namespace Lsp.Tests.Integration.Fixtures
{
    public interface IConfigureLanguageProtocolFixture
    {
        JsonRpcTestOptions Configure(JsonRpcTestOptions options);
    }
}