using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

#pragma warning disable 618

namespace Lsp.Tests
{
    public class DocumentSymbolKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialSymbolKinds()
        {
            var serializer = new LspSerializer();
            var json = serializer.SerializeObject(
                new SymbolInformation {
                    Kind = SymbolKind.Event
                }
            );

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Kind.Should().Be(SymbolKind.Event);
        }

        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialSymbolTags()
        {
            var serializer = new LspSerializer();
            var json = serializer.SerializeObject(
                new SymbolInformation {
                    Tags = new Container<SymbolTag>(SymbolTag.Deprecated)
                }
            );

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Tags.Should().Contain(SymbolTag.Deprecated);
        }

        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Client()
        {
            var serializer = new LspSerializer();
            serializer.SetClientCapabilities(
                new ClientCapabilities {
                    TextDocument = new TextDocumentClientCapabilities {
                        DocumentSymbol = new Supports<DocumentSymbolCapability?>(
                            true, new DocumentSymbolCapability {
                                DynamicRegistration = true,
                                SymbolKind = new SymbolKindCapabilityOptions {
                                    ValueSet = new Container<SymbolKind>(SymbolKind.Class)
                                }
                            }
                        )
                    }
                }
            );

            var json = serializer.SerializeObject(
                new DocumentSymbol {
                    Kind = SymbolKind.Event
                }
            );

            var result = serializer.DeserializeObject<DocumentSymbol>(json);
            result.Kind.Should().Be(SymbolKind.Class);
        }
    }
}
