using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests
{
    public class CodeActionKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialSymbolKinds()
        {
            var json = JsonSerializer.Serialize(new CodeAction()
            {
                Kind = CodeActionKind.Source
            }, Serializer.Instance.Options);

            var result = JsonSerializer.Deserialize<CodeAction>(json, Serializer.Instance.Options);
            result.Kind.Should().Be(CodeActionKind.Source);
        }

        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(ClientVersion.Lsp3, new ClientCapabilities()
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    CodeAction = new Supports<CodeActionCapability>(true, new CodeActionCapability()
                    {
                        DynamicRegistration = true,
                        CodeActionLiteralSupport = new CodeActionLiteralSupportCapability() {
                            CodeActionKind = new CodeActionKindCapability() {
                                ValueSet = new Container<CodeActionKind>(CodeActionKind.RefactorInline)
                            }
                        }
                    })
                }
            });

            var json = JsonSerializer.Serialize(new CodeAction()
            {
                Kind = CodeActionKind.QuickFix
            }, serializer.Options);

            var result = JsonSerializer.Deserialize<CodeAction>(json, serializer.Options);
            result.Kind.Should().Be(CodeActionKind.RefactorInline);
        }
    }
}
