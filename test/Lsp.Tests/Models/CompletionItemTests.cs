using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CompletionItemTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CompletionItem()
            {
                Kind = CompletionItemKind.Text,
                CommitCharacters = new[] { ";", "/", "." },
                AdditionalTextEdits = new[] {
                    new TextEdit() {
                        NewText = "new text"
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CompletionItem>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
