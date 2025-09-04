using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;

namespace Lsp.Tests.Models
{
    public class CompletionListTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionList(
                new List<CompletionItem> {
                    new CompletionItem {
                        Kind = CompletionItemKind.Class,
                        Detail = "details"
                    }
                }
            );
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionList>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Theory]
        [JsonFixture]
        public void ComplexTest(string expected)
        {
            var model = new CompletionList(
                new List<CompletionItem> {
                    new CompletionItem {
                        Kind = CompletionItemKind.Class,
                        Detail = "details"
                    }
                }, true
            );
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<CompletionList>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
