using System;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Capabilities.Client
{
    public class TextDocumentClientCapabilitiesTests : AutoTestBase
    {
        public TextDocumentClientCapabilitiesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentClientCapabilities()
            {
                CodeAction = new CodeActionClientCapabilities() { DynamicRegistration = true },
                CodeLens = new CodeLensClientCapabilities() { DynamicRegistration = true },
                Definition = new DefinitionClientCapabilities() { DynamicRegistration = true },
                DocumentHighlight = new DocumentHighlightClientCapabilities() { DynamicRegistration = true },
                DocumentLink = new DocumentLinkClientCapabilities() { DynamicRegistration = true },
                DocumentSymbol = new DocumentSymbolClientCapabilities() { DynamicRegistration = true },
                Formatting = new DocumentFormattingClientCapabilities() { DynamicRegistration = true },
                Hover = new HoverClientCapabilities() { DynamicRegistration = true },
                OnTypeFormatting = new DocumentOnTypeFormattingClientCapabilities() { DynamicRegistration = true },
                RangeFormatting = new DocumentRangeFormattingClientCapabilities() { DynamicRegistration = true },
                References = new ReferenceClientCapabilities() { DynamicRegistration = true },
                Rename = new RenameClientCapabilities() { DynamicRegistration = true },
                SignatureHelp = new SignatureHelpClientCapabilities() { DynamicRegistration = true },
                Completion = new CompletionClientCapabilities()
                {
                    DynamicRegistration = true,
                    CompletionItem = new CompletionItemClientCapabilities()
                    {
                        SnippetSupport = true
                    }
                },
                Synchronization = new TextDocumentSyncClientCapabilities()
                {
                    DynamicRegistration = true,
                    WillSave = true,
                    DidSave = true,
                    WillSaveWaitUntil = true
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentClientCapabilities>(expected);
            deresult.Should().BeEquivalentTo(model, o => o.ConfigureForSupports(Logger));
        }

        [Theory, JsonFixture]
        public void EmptyTest(string expected)
        {
            var model = new TextDocumentClientCapabilities();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentClientCapabilities>(expected);
            deresult.Should().BeEquivalentTo(model, o => o.ConfigureForSupports(Logger));
        }
    }
}
