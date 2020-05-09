using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Models
{
    public class CodeActionParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeActionParams() {
                Context = new CodeActionContext() {
                    Diagnostics = new[] { new Diagnostic() {
                    Code = new DiagnosticCode("abcd"),
                    Message = "message",
                    Range = new Range(new Position(1, 1), new Position(2,2)),
                    Severity = DiagnosticSeverity.Error,
                    Source = "csharp"
                } }

                },
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                TextDocument = new TextDocumentIdentifier() {
                    Uri = new Uri("file:///test/123/d.cs")
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CodeActionParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void NonStandardCharactersTest(string expected)
        {
            var model = new CodeActionParams() {
                Context = new CodeActionContext() {
                    Diagnostics = new[] { new Diagnostic() {
                    Code = new DiagnosticCode("abcd"),
                    Message = "message",
                    Range = new Range(new Position(1, 1), new Position(2,2)),
                    Severity = DiagnosticSeverity.Error,
                    Source = "csharp"
                } }

                },
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                TextDocument = new TextDocumentIdentifier() {
                    // 树 - Chinese for tree
                    Uri = new Uri("file:///test/123/树.cs")
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<CodeActionParams>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
