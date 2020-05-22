using System.Linq;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class DocumentOnTypeFormattingOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentOnTypeFormattingOptions() {
                FirstTriggerCharacter = ".",
                MoreTriggerCharacter = ";`".Select(x => x.ToString()).ToArray(),
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DocumentOnTypeFormattingOptions>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void Optional(string expected)
        {
            var model = new DocumentOnTypeFormattingOptions() {
                FirstTriggerCharacter = "."
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DocumentOnTypeFormattingOptions>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
