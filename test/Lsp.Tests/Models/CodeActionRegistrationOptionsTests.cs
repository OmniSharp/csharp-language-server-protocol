using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeActionRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeActionRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new[] { new DocumentFilter(){
                    Language = "csharp",
                    Pattern = "pattern",
                    Scheme = "scheme"
                }, new DocumentFilter(){
                    Language = "vb",
                    Pattern = "pattern",
                    Scheme = "scheme"
                } }),
                CodeActionKinds = new[] {
                    CodeActionKind.QuickFix,
                    CodeActionKind.Refactor,
                    CodeActionKind.RefactorExtract,
                    CodeActionKind.RefactorInline,
                    CodeActionKind.RefactorRewrite,
                    CodeActionKind.Source,
                    CodeActionKind.SourceOrganizeImports
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CodeActionRegistrationOptions>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
