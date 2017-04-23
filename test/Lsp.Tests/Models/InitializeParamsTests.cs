using System;
using System.Collections.Generic;
using FluentAssertions;
using Lsp.Capabilities.Client;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class InitializeParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InitializeParams() {
                Capabilities = new ClientCapabilities() {
                    Experimental = new Dictionary<string, object>()
                {
                    {  "abc", "test" }
                },
                    TextDocument = new TextDocumentClientCapabilities() {
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
                    },
                    Workspace = new WorkspaceClientCapabilites() {
                        ApplyEdit = true,
                        DidChangeConfiguration = new DidChangeConfigurationCapability() { DynamicRegistration = true },
                        DidChangeWatchedFiles = new DidChangeWatchedFilesCapability() { DynamicRegistration = true },
                        ExecuteCommand = new ExecuteCommandCapability() { DynamicRegistration = true },
                        Symbol = new WorkspaceSymbolCapability() { DynamicRegistration = true },
                    }
                },
                InitializationOptions = null,
                ProcessId = 1234,
                RootUri = new Uri("file:///file/abc/12.cs"),
                Trace = InitializeTrace.verbose
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<InitializeParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
