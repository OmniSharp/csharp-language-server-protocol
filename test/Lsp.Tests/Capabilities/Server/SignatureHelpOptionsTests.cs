using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class SignatureHelpOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new SignatureHelpOptions() {
                TriggerCharacters = new[] { "1", "2" }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<SignatureHelpOptions>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
