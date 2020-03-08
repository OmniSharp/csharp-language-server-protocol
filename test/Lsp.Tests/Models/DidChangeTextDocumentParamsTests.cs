using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidChangeTextDocumentParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void NonStandardCharactersTest(string expected)
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
                    Uri = new Uri("C:\\abc\\Mörkö.cs")
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<DidChangeTextDocumentParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
