using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class TextDocumentClientCapabilitiesTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentClientCapabilities() {
                CodeAction = new CodeActionCapability() { DynamicRegistration = true },
                CodeLens = new CodeLensCapability() { DynamicRegistration = true },
                Definition = new DefinitionCapability() { DynamicRegistration = true },
                DocumentHighlight = new DocumentHighlightCapability() { DynamicRegistration = true },
                DocumentLink = new DocumentLinkCapability() { DynamicRegistration = true },
                DocumentSymbol = new DocumentSymbolCapability() { DynamicRegistration = true },
                Formatting = new DocumentFormattingCapability() { DynamicRegistration = true },
                Hover = new HoverCapability() { DynamicRegistration = true },
                OnTypeFormatting = new DocumentOnTypeFormattingCapability() { DynamicRegistration = true },
                RangeFormatting = new DocumentRangeFormattingCapability() { DynamicRegistration = true },
                References = new ReferencesCapability() { DynamicRegistration = true },
                Rename = new RenameCapability() { DynamicRegistration = true },
                SignatureHelp = new SignatureHelpCapability() { DynamicRegistration = true },
                Completion = new CompletionCapability() {
                    DynamicRegistration = true,
                    CompletionItem = new CompletionItemCapability() {
                        SnippetSupport = true
                    }
                },
                Synchronization = new SynchronizationCapability() {
                    DynamicRegistration = true,
                    WillSave = true,
                    DidSave = true,
                    WillSaveWaitUntil = true
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentClientCapabilities>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
