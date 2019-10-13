using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
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
                        },
                        FoldingRange = new FoldingRangeClientCapabilities
                        {
                            DynamicRegistration = true,
                            LineFoldingOnly = true,
                            RangeLimit = 5000,
                        }
                    },
                    Workspace = new WorkspaceClientCapabilities()
                    {
                        ApplyEdit = true,
                        DidChangeConfiguration = new DidChangeConfigurationClientCapabilities() { DynamicRegistration = true },
                        DidChangeWatchedFiles = new DidChangeWatchedFilesClientCapabilities() { DynamicRegistration = true },
                        ExecuteCommand = new ExecuteCommandClientCapabilities() { DynamicRegistration = true },
                        Symbol = new WorkspaceSymbolClientCapabilities() { DynamicRegistration = true },

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
