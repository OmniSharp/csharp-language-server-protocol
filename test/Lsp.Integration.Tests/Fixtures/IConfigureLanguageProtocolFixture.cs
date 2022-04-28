using OmniSharp.Extensions.JsonRpc.Testing;

namespace Lsp.Integration.Tests.Fixtures
{
    public interface IConfigureLanguageProtocolFixture
    {
        JsonRpcTestOptions Configure(JsonRpcTestOptions options);
    }
}
