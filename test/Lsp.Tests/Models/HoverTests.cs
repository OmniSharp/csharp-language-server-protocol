using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Models
{
    public class HoverTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Hover() {
                Contents = new MarkedStringsOrMarkupContent("abc"),
                Range = new Range(new Position(1, 2), new Position(3, 4))
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Hover>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
