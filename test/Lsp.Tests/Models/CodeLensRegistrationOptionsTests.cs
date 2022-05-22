using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeLensRegistrationOptionsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeLensRegistrationOptions {
                DocumentSelector = new TextDocumentSelector(
                    new TextDocumentFilter {
                        Language = "csharp",
                        Pattern = "pattern",
                        Scheme = "scheme"
                    }, new TextDocumentFilter {
                        Language = "vb",
                        Pattern = "pattern",
                        Scheme = "scheme"
                    }
                ),
                ResolveProvider = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CodeLensRegistrationOptions>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
