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
            var serializer = new Serializer();
            var json = serializer.SerializeObject(
                new Diagnostic {
                    Tags = new Container<DiagnosticTag>(DiagnosticTag.Deprecated)
                }
            );

            var result = serializer.DeserializeObject<Diagnostic>(json);
            result.Tags.Should().Contain(DiagnosticTag.Deprecated);
        }

        [Fact]
        public void CustomBehavior_When_DiagnosticTag_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(
                ClientVersion.Lsp3, new ClientCapabilities {
                    TextDocument = new TextDocumentClientCapabilities {
                        PublishDiagnostics = new Supports<PublishDiagnosticsCapability>(
                            true, new PublishDiagnosticsCapability {
                                TagSupport = new PublishDiagnosticsTagSupportCapabilityOptions {
                                    ValueSet = new Container<DiagnosticTag>()
                                }
                            }
                        )
                    }
                }
            );

            var json = serializer.SerializeObject(
                new Diagnostic {
                    Tags = new Container<DiagnosticTag>(DiagnosticTag.Deprecated)
                }
            );

            var result = serializer.DeserializeObject<Diagnostic>(json);
            result.Tags.Should().BeEmpty();
        }
    }
}
