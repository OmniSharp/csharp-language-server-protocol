using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeTextDocumentParamsTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeTextDocumentParams {
                ContentChanges = new[] {
                    new TextDocumentContentChangeEvent {
                        Range = new Range(new Position(1, 1), new Position(2, 2)),
                        RangeLength = 12,
                        Text = "abc"
                    }
                },
                TextDocument = new OptionalVersionedTextDocumentIdentifier {
                    Uri = "/somepath/to/a/file.ext",
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<DidChangeTextDocumentParams>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Theory]
        [JsonFixture]
        public void NonStandardCharactersTest(string expected)
        {
            var model = new DidChangeTextDocumentParams {
                ContentChanges = new[] {
                    new TextDocumentContentChangeEvent {
                        Range = new Range(new Position(1, 1), new Position(2, 2)),
                        RangeLength = 12,
                        Text = "abc"
                    }
                },
                TextDocument = new OptionalVersionedTextDocumentIdentifier {
                    Uri = DocumentUri.FromFileSystemPath("c:\\abc\\Mörkö.cs")
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<DidChangeTextDocumentParams>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
