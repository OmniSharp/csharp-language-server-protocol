using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentOnTypeFormattingRegistrationOptionsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentOnTypeFormattingRegistrationOptions {
                DocumentSelector = new TextDocumentSelector(
                    new TextDocumentFilter {
                        Language = "csharp"
                    }
                ),
                FirstTriggerCharacter = "1",
                MoreTriggerCharacter = new[] { "1", "2" }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<DocumentOnTypeFormattingRegistrationOptions>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
