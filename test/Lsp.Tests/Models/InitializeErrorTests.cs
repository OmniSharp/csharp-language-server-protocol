using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class InitializeErrorTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InitializeError();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<InitializeError>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
