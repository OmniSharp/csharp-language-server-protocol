using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Lsp.Tests
{
    public class CodeActionKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialSymbolKinds()
        {
            var serializer = new LspSerializer();
            var json = serializer.SerializeObject(
                new CodeAction {
                    Kind = CodeActionKind.Source
                }
            );

            var result = serializer.DeserializeObject<CodeAction>(json);
            result.Kind.Should().Be(CodeActionKind.Source);
        }

        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Client()
        {
            var serializer = new LspSerializer();
            serializer.SetClientCapabilities(
                new ClientCapabilities {
                    TextDocument = new TextDocumentClientCapabilities {
                        CodeAction = new Supports<CodeActionCapability?>(
                            true, new CodeActionCapability {
                                DynamicRegistration = true,
                                CodeActionLiteralSupport = new CodeActionLiteralSupportOptions {
                                    CodeActionKind = new CodeActionKindCapabilityOptions {
                                        ValueSet = new Container<CodeActionKind>(CodeActionKind.RefactorInline)
                                    }
                                }
                            }
                        )
                    }
                }
            );

            var json = serializer.SerializeObject(
                new CodeAction {
                    Kind = CodeActionKind.QuickFix
                }
            );

            var result = serializer.DeserializeObject<CodeAction>(json);
            result.Kind.Should().Be(CodeActionKind.RefactorInline);
        }

        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Server()
        {
            var serializer = new LspSerializer();
            serializer.SetServerCapabilities(
                new ServerCapabilities {
                    CodeActionProvider = new CodeActionRegistrationOptions.StaticOptions() {
                        CodeActionKinds = new Container<CodeActionKind>(CodeActionKind.RefactorInline)
                    },
                }
            );

            var json = serializer.SerializeObject(
                new CodeAction {
                    Kind = CodeActionKind.QuickFix
                }
            );

            var result = serializer.DeserializeObject<CodeAction>(json);
            result.Kind.Should().Be(CodeActionKind.RefactorInline);
        }
    }
}
