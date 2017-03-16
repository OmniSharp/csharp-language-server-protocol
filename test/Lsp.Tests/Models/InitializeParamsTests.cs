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
                    },
                    Workspace = new WorkspaceClientCapabilites() {
                        ApplyEdit = true,
                        DidChangeConfiguration = new DynamicCapability() { DynamicRegistration = true },
                        DidChangeWatchedFiles = new DynamicCapability() { DynamicRegistration = true },
                        ExecuteCommand = new DynamicCapability() { DynamicRegistration = true },
                        Symbol = new DynamicCapability() { DynamicRegistration = true },
                    }
                },
                InitializationOptions = null,
                ProcessId = 1234,
                RootUri = new Uri("file:///file/abc/12.cs"),
                Trace = InitializeParams.InitializeTrace.verbose
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<InitializeParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
