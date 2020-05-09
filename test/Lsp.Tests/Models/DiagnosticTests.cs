using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Tests.Models
{
    public class DiagnosticTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new Diagnostic() {
                Code = new DiagnosticCode("abcd"),
                Message = "message",
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                Severity = DiagnosticSeverity.Error,
                Source = "csharp"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Diagnostic>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void RelatedInformationTest(string expected)
        {
            var model = new Diagnostic() {
                Code = new DiagnosticCode("abcd"),
                Message = "message",
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                Severity = DiagnosticSeverity.Error,
                Source = "csharp",
                RelatedInformation = new Container<DiagnosticRelatedInformation>(
                    new DiagnosticRelatedInformation {
                        Location = new Location {
                            Uri = new Uri("file:///abc/def.cs"),
                            Range = new Range(new Position(1, 2), new Position(3, 4))
                        },
                        Message = "related message 1"
                    },
                    new DiagnosticRelatedInformation {
                        Location = new Location {
                            Uri = new Uri("file:///def/ghi.cs"),
                            Range = new Range(new Position(5, 6), new Position(7, 8))
                        },
                        Message = "related message 2"
                    }
                )
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Diagnostic>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void OptionalTest(string expected)
        {
            var model = new Diagnostic();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<Diagnostic>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
