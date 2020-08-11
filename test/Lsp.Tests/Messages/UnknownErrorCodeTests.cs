using FluentAssertions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server.Messages;
using Xunit;

namespace Lsp.Tests.Messages
{
    public class UnknownErrorCodeTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new UnknownErrorCode("");
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<RpcError>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
