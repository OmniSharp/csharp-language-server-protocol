using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests
{
    public class DocumentSymbolKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialKinds()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(new SymbolInformation()
            {
                Kind = SymbolKind.Event
            });

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Kind.Should().Be(SymbolKind.File);
        }

        [Fact]
        public void CustomBehavior_When_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(ClientVersion.Lsp3, new ClientCapabilities()
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    DocumentSymbol = new Supports<DocumentSymbolClientCapabilities>(true, new DocumentSymbolClientCapabilities()
                    {
                        DynamicRegistration = true,
                        SymbolKind = new SymbolKindClientCapabilities()
                        {
                            ValueSet = new Container<SymbolKind>(SymbolKind.Class)
                        }
                    })
                }
            });

            var json = serializer.SerializeObject(new SymbolInformation()
            {
                Kind = SymbolKind.Event
            });

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Kind.Should().Be(SymbolKind.Class);
        }
    }
}
