using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Models
{
    public class RangeTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Range(new Position(1, 1), new Position(2, 2));
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Range>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
