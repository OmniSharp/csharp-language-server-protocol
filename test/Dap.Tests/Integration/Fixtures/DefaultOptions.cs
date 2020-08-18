using OmniSharp.Extensions.JsonRpc.Testing;

namespace Dap.Tests.Integration.Fixtures
{
    public sealed class DefaultOptions : IConfigureDebugAdapterProtocolFixture
    {
        public JsonRpcTestOptions Configure(JsonRpcTestOptions options) => options;
    }
}