using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeTextDocumentParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeTextDocumentParams() {
                ContentChanges = new[] {
                    new TextDocumentContentChangeEvent() {
                        Range = new Range(new Position(1,1), new Position(2, 2)),
                        RangeLength = 12,
                        Text = "abc"
                    }
                },
                TextDocument = new VersionedTextDocumentIdentifier() {

                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DidChangeTextDocumentParams>(expected, Serializer.CreateSerializerSettings(ClientVersion.Lsp3));
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
