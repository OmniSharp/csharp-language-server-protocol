using System.Collections.Immutable;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DiagnosticReportTests
    {
        private readonly LspSerializer _serializer = new(ClientVersion.Lsp3);

        [Fact]
        public void Should_RoundTrip_Document_Diagnostic_Report()
        {
            DocumentDiagnosticReport model = new FullDocumentDiagnosticReport {
                ResultId = "result-1",
                Items = new Container<Diagnostic>(
                    new Diagnostic {
                        Message = "message",
                        Range = new Range(new Position(1, 2), new Position(3, 4))
                    }
                )
            };

            var result = _serializer.SerializeObject(model);

            result.Should().Be(@"{""kind"":""full"",""resultId"":""result-1"",""items"":[{""range"":{""start"":{""line"":1,""character"":2},""end"":{""line"":3,""character"":4}},""message"":""message""}]}");

            var deresult = _serializer.DeserializeObject<DocumentDiagnosticReport>(result);
            deresult.Should().BeOfType<FullDocumentDiagnosticReport>();
            var full = (FullDocumentDiagnosticReport)deresult;
            full.ResultId.Should().Be("result-1");
            full.Items.Should().ContainSingle().Which.Message.String.Should().Be("message");
        }

        [Fact]
        public void Should_RoundTrip_Related_Document_Diagnostic_Report()
        {
            var relatedUri = DocumentUri.Parse("file:///related.cs");
            RelatedDocumentDiagnosticReport model = new RelatedUnchangedDocumentDiagnosticReport {
                ResultId = "result-2",
                RelatedDocuments = ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>.Empty.Add(
                    relatedUri,
                    new FullDocumentDiagnosticReport {
                        Items = new Container<Diagnostic>()
                    }
                )
            };

            var result = _serializer.SerializeObject(model);

            var json = JObject.Parse(result);
            json["kind"]!.Value<string>().Should().Be("unchanged");
            json["resultId"]!.Value<string>().Should().Be("result-2");
            json["relatedDocuments"]![relatedUri.ToString()]!["kind"]!.Value<string>().Should().Be("full");

            var deresult = _serializer.DeserializeObject<RelatedDocumentDiagnosticReport>(result);
            deresult.Should().BeOfType<RelatedUnchangedDocumentDiagnosticReport>();
            deresult.RelatedDocuments.Should().ContainKey(relatedUri);
            deresult.RelatedDocuments![relatedUri].Should().BeOfType<FullDocumentDiagnosticReport>();
        }

        [Fact]
        public void Should_RoundTrip_Workspace_Diagnostic_Report_Items()
        {
            WorkspaceDocumentDiagnosticReport model = new WorkspaceFullDocumentDiagnosticReport {
                Uri = DocumentUri.Parse("file:///workspace.cs"),
                Version = null,
                ResultId = "workspace-result",
                Items = new Container<Diagnostic>()
            };

            var result = _serializer.SerializeObject(model);

            result.Should().Be(@"{""kind"":""full"",""uri"":""file:///workspace.cs"",""version"":null,""resultId"":""workspace-result"",""items"":[]}");

            var deresult = _serializer.DeserializeObject<WorkspaceDocumentDiagnosticReport>(result);
            deresult.Should().BeOfType<WorkspaceFullDocumentDiagnosticReport>();
            var full = (WorkspaceFullDocumentDiagnosticReport)deresult;
            full.Uri.Should().Be(DocumentUri.Parse("file:///workspace.cs"));
            full.Version.Should().BeNull();
            full.ResultId.Should().Be("workspace-result");
        }

        [Fact]
        public void Should_Create_Partial_Result_From_Related_Report()
        {
            var uri = DocumentUri.Parse("file:///related.cs");
            var report = new RelatedFullDocumentDiagnosticReport {
                Items = new Container<Diagnostic>(),
                RelatedDocuments = ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>.Empty.Add(
                    uri,
                    new UnchangedDocumentDiagnosticReport { ResultId = "same" }
                )
            };

            var partial = DocumentDiagnosticReportPartialResult.From(report);

            partial.Should().NotBeNull();
            partial!.RelatedDocuments.Should().ContainKey(uri);
            partial.RelatedDocuments![uri].Should().BeOfType<UnchangedDocumentDiagnosticReport>();
        }
    }
}
