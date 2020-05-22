using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class PositionTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Position(1, 1);
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Position>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
