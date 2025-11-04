using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;

namespace Lsp.Tests.Models
{
    public class DocumentLinkRegistrationOptionsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DocumentLinkRegistrationOptions {
                DocumentSelector = new TextDocumentSelector(
                    new TextDocumentFilter {
                        Language = "csharp"
                    }
                ),
                ResolveProvider = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<DocumentLinkRegistrationOptions>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
