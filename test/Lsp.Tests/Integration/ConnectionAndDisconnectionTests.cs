using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class ConnectionAndDisconnectionTests : LanguageProtocolTestBase
    {
        public ConnectionAndDisconnectionTests(ITestOutputHelper outputHelper)  : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public void Test123()
        {
            
        }
    }
}
