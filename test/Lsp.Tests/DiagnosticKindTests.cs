using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests
{
    public class DiagnosticKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialDiagnosticTags()
        {

            var json = JsonSerializer.Serialize(new Diagnostic()
            {
                Tags = new Container<DiagnosticTag>(DiagnosticTag.Deprecated)
            }, Serializer.Instance.Options);

            var result = JsonSerializer.Deserialize<Diagnostic>(json, Serializer.Instance.Options);
            result.Tags.Should().Contain(DiagnosticTag.Deprecated);
        }

        [Fact]
        public void CustomBehavior_When_DiagnosticTag_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(ClientVersion.Lsp3, new ClientCapabilities()
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    PublishDiagnostics = new Supports<PublishDiagnosticsCapability>(true, new PublishDiagnosticsCapability()
                    {
                        TagSupport = new PublishDiagnosticsTagSupportCapability() {
                            ValueSet = new Container<DiagnosticTag>()
                        }
                    })
                }
            });

            var json = JsonSerializer.Serialize(new Diagnostic()
            {
                Tags = new Container<DiagnosticTag>(DiagnosticTag.Deprecated)
            }, serializer.Options);

            var result = JsonSerializer.Deserialize<Diagnostic>(json, serializer.Options);
            result.Tags.Should().BeEmpty();
        }
    }
}
