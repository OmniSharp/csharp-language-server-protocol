using OmniSharp.Extensions.JsonRpc.Testing;

namespace Lsp.Tests.Integration.Fixtures
{
    public sealed class DefaultOptions : IConfigureLanguageProtocolFixture
    {
        public JsonRpcTestOptions Configure(JsonRpcTestOptions options) => options;
    }
}