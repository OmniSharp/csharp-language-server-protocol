using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextEditTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextEdit {
                NewText = "new text",
                Range = new Range(new Position(1, 1), new Position(2, 2))
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<TextEdit>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Theory]
        [JsonFixture]
        public void AnnotatedTest(string expected)
        {
            var model = new AnnotatedTextEdit {
                NewText = "new text",
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                AnnotationId = "Cool story"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<TextEdit>(expected);
            deresult.Should().BeOfType<AnnotatedTextEdit>();
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
