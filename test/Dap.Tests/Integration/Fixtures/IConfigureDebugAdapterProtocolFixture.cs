using OmniSharp.Extensions.JsonRpc.Testing;

namespace Dap.Tests.Integration.Fixtures
{
    public interface IConfigureDebugAdapterProtocolFixture
    {
        JsonRpcTestOptions Configure(JsonRpcTestOptions options);
    }
}