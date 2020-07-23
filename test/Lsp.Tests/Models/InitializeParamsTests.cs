using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Models
{
    public class InitializeParamsTests : AutoTestBase
    {
        public InitializeParamsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InitializeParams()
            {
                Capabilities = new ClientCapabilities()
                {
                    Experimental = new Dictionary<string, JToken>() { { "abc", "test" } },
                    TextDocument = new TextDocumentClientCapabilities()
                    {
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
                        References = new ReferenceCapability() { DynamicRegistration = true },
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
                        },
                        FoldingRange = new FoldingRangeCapability
                        {
                            DynamicRegistration = true,
                            LineFoldingOnly = true,
                            RangeLimit = 5000,
                        }
                    },
                    Workspace = new WorkspaceClientCapabilities()
                    {
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
                Trace = InitializeTrace.Verbose
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<InitializeParams>(expected);
            deresult.Should().BeEquivalentTo(model, o => o.ConfigureForSupports(Logger));
        }
    }
}
