using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class ClientCapabilitiesTests
    {
        // private const Fixtures =
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ClientCapabilities()
            {
                Experimental = new Dictionary<string, JToken>()
                {
                    {  "abc", "test" }
                },
                TextDocument = new TextDocumentClientCapabilities()
                {
                    CodeAction = new CodeActionCapability() {  DynamicRegistration = true },
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
                    Completion = new CompletionCapability()
                    {
                        DynamicRegistration = true,
                        CompletionItem = new CompletionItemCapability()
                        {
                            SnippetSupport = true
                        }
                    },
                    Synchronization = new SynchronizationCapability()
                    {
                        DynamicRegistration = true,
                        WillSave = true,
                        DidSave = true,
                        WillSaveWaitUntil = true
                    }
                },
                Workspace = new WorkspaceClientCapabilites()
                {
                    ApplyEdit = true,
                    WorkspaceEdit = new WorkspaceEditCapability() { DocumentChanges = true },
                    DidChangeConfiguration = new DidChangeConfigurationCapability() { DynamicRegistration = true },
                    DidChangeWatchedFiles = new DidChangeWatchedFilesCapability() { DynamicRegistration = true },
                    ExecuteCommand = new ExecuteCommandCapability() { DynamicRegistration = true },
                    Symbol = new WorkspaceSymbolCapability() { DynamicRegistration = true },
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<ClientCapabilities>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
