using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests
{
    public class CompletionItemKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialKinds()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(new CompletionItem()
            {
                Kind = CompletionItemKind.Event
            });

            var result = serializer.DeserializeObject<CompletionItem>(json);
            result.Kind.Should().Be(CompletionItemKind.Text);
        }

        [Fact]
        public void CustomBehavior_When_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(ClientVersion.Lsp3, new ClientCapabilities()
            {
                TextDocument = new TextDocumentClientCapabilities
                {
                    Completion = new Supports<CompletionCapability>(true, new CompletionCapability()
                    {
                        DynamicRegistration = true,
                        CompletionItemKind = new CompletionItemKindCapability()
                        {
                            ValueSet = new Container<CompletionItemKind>(CompletionItemKind.Class)
                        }
                    })
                }
            });

            var json = serializer.SerializeObject(new CompletionItem()
            {
                Kind = CompletionItemKind.Event
            });

            var result = serializer.DeserializeObject<CompletionItem>(json);
            result.Kind.Should().Be(CompletionItemKind.Class);
        }
    }
}
