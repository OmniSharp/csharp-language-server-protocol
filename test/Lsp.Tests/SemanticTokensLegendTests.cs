using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Lsp.Tests
{
    public class SemanticTokensLegendTests
    {
        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Client()
        {
            var serializer = new LspSerializer();
            serializer.SetClientCapabilities(
                new ClientCapabilities
                {
                    TextDocument = new TextDocumentClientCapabilities
                    {
                        SemanticTokens = new SemanticTokensCapability
                        {
                            DynamicRegistration = true,
                            Formats = new Container<SemanticTokenFormat>(SemanticTokenFormat.Relative),
                            MultilineTokenSupport = true,
                            OverlappingTokenSupport = true,
                            TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier.Deprecated),
                            TokenTypes = new Container<SemanticTokenType>(SemanticTokenType.Comment),
                            Requests = new SemanticTokensCapabilityRequests
                            {
                                Full = new SemanticTokensCapabilityRequestFull
                                {
                                    Delta = true
                                },
                                Range = new SemanticTokensCapabilityRequestRange()
                            }
                        },
                    }
                }
            );

            var json = serializer.SerializeObject(
                new SemanticTokensLegend
                {
                    TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier.Deprecated),
                    TokenTypes = new Container<SemanticTokenType>(SemanticTokenType.Comment),
                }
            );

            var result = serializer.DeserializeObject<SemanticTokensLegend>(json);
            result.TokenModifiers.FirstOrDefault().Should().Be(SemanticTokenModifier.Deprecated);
            result.TokenTypes.FirstOrDefault().Should().Be(SemanticTokenType.Comment);
        }

        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Server()
        {
            var serializer = new LspSerializer();
            serializer.SetServerCapabilities(
                new ServerCapabilities
                {
                    SemanticTokensProvider = new SemanticTokensRegistrationOptions.StaticOptions
                    {
                        Full = new SemanticTokensCapabilityRequestFull
                        {
                            Delta = true
                        },
                        Legend = new SemanticTokensLegend
                        {
                            TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier.Deprecated),
                            TokenTypes = new Container<SemanticTokenType>(SemanticTokenType.Comment),
                        },
                        Range = new SemanticTokensCapabilityRequestRange(),
                    }
                }
            );

            var json = serializer.SerializeObject(
                new SemanticTokensLegend
                {
                    TokenModifiers = new Container<SemanticTokenModifier>(SemanticTokenModifier.Deprecated),
                    TokenTypes = new Container<SemanticTokenType>(SemanticTokenType.Comment),
                }
            );

            var result = serializer.DeserializeObject<SemanticTokensLegend>(json);
            result.TokenModifiers.FirstOrDefault().Should().Be(SemanticTokenModifier.Deprecated);
            result.TokenTypes.FirstOrDefault().Should().Be(SemanticTokenType.Comment);
        }
    }
}
