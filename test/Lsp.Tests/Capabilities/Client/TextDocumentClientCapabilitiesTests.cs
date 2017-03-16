using System;
using FluentAssertions;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class TextDocumentClientCapabilitiesTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentClientCapabilities() {
                CodeAction = new DynamicCapability() { DynamicRegistration = true },
                CodeLens = new DynamicCapability() { DynamicRegistration = true },
                Definition = new DynamicCapability() { DynamicRegistration = true },
                DocumentHighlight = new DynamicCapability() { DynamicRegistration = true },
                DocumentLink = new DynamicCapability() { DynamicRegistration = true },
                DocumentSymbol = new DynamicCapability() { DynamicRegistration = true },
                Formatting = new DynamicCapability() { DynamicRegistration = true },
                Hover = new DynamicCapability() { DynamicRegistration = true },
                OnTypeFormatting = new DynamicCapability() { DynamicRegistration = true },
                RangeFormatting = new DynamicCapability() { DynamicRegistration = true },
                References = new DynamicCapability() { DynamicRegistration = true },
                Rename = new DynamicCapability() { DynamicRegistration = true },
                SignatureHelp = new DynamicCapability() { DynamicRegistration = true },
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
