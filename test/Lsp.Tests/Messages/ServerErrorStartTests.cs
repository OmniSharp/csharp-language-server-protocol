using FluentAssertions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server.Messages;
using Serializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer;
using Xunit;

namespace Lsp.Tests.Messages
{
    public class ServerErrorStartTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ServerErrorStart("abcd");
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<RpcError>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
