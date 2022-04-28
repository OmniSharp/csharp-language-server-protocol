using OmniSharp.Extensions.JsonRpc.Testing;

namespace Lsp.Integration.Tests.Fixtures
{
    public sealed class DefaultOptions : IConfigureLanguageProtocolFixture
    {
        public JsonRpcTestOptions Configure(JsonRpcTestOptions options)
        {
            return options;
        }
    }
}
