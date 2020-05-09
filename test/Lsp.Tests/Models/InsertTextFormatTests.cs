using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class InsertTextFormatTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InsertTextFormat();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<InsertTextFormat>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
