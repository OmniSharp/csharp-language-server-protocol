using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;


namespace Lsp.Tests
{
    public class CompletionItemKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialCompletionItemKinds()
        {
            var serializer = new LspSerializer();
            var json = serializer.SerializeObject(
                new CompletionItem {
                    Kind = CompletionItemKind.Event
                }
            );

            var result = serializer.DeserializeObject<CompletionItem>(json);
            result.Kind.Should().Be(CompletionItemKind.Event);
        }

        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialCompletionItemTags()
        {
            var serializer = new LspSerializer();
            var json = serializer.SerializeObject(
                new CompletionItem {
                    Tags = new Container<CompletionItemTag>(CompletionItemTag.Deprecated)
                }
            );

            var result = serializer.DeserializeObject<CompletionItem>(json);
            result.Tags.Should().Contain(CompletionItemTag.Deprecated);
        }

        [Fact]
        public void CustomBehavior_When_CompletionItemKinds_Defined_By_Client()
        {
            var serializer = new LspSerializer();
            serializer.SetClientCapabilities(
                new ClientCapabilities {
                    TextDocument = new TextDocumentClientCapabilities {
                        Completion = new Supports<CompletionCapability?>(
                            true, new CompletionCapability {
                                DynamicRegistration = true,
                                CompletionItemKind = new CompletionItemKindCapabilityOptions {
                                    ValueSet = new Container<CompletionItemKind>(CompletionItemKind.Class)
                                }
                            }
                        )
                    }
                }
            );

            var json = serializer.SerializeObject(
                new CompletionItem {
                    Kind = CompletionItemKind.Event
                }
            );

            var result = serializer.DeserializeObject<CompletionItem>(json);
            result.Kind.Should().Be(CompletionItemKind.Class);
        }
    }
}
