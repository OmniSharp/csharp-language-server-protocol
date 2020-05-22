using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Capabilities.Client
{
    public class ClientCapabilitiesTests : AutoTestBase
    {
        // private const Fixtures =
        public ClientCapabilitiesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

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
                    CodeAction = new CodeActionCapability() { DynamicRegistration = true },
                    CodeLens = new CodeLensCapability() { DynamicRegistration = true },
                    Definition = new DefinitionCapability() { DynamicRegistration = true, LinkSupport = true },
                    Declaration = new DeclarationCapability() { DynamicRegistration = true, LinkSupport = true },
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
                    Implementation = new ImplementationCapability()
                    {
                        DynamicRegistration = true,
                        LinkSupport = true
                    },
                    TypeDefinition = new TypeDefinitionCapability()
                    {
                        DynamicRegistration = true,
                        LinkSupport = true
                    },
                    Synchronization = new SynchronizationCapability()
                    {
                        DynamicRegistration = true,
                        WillSave = true,
                        DidSave = true,
                        WillSaveWaitUntil = true
                    },
                    FoldingRange = new FoldingRangeCapability() {
                        DynamicRegistration = true,
                        LineFoldingOnly = true,
                        RangeLimit = 5000
                    },
                    SelectionRange = new SelectionRangeCapability() {
                        DynamicRegistration = true,
                        LineFoldingOnly = true,
                        RangeLimit = 5000
                    }

                },
                Workspace = new WorkspaceClientCapabilities()
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
            deresult.Should().BeEquivalentTo(model, o => o
                .ConfigureForSupports(Logger));
        }

        [Theory, JsonFixture]
        public void Github_Issue_75(string expected)
        {
            Action a = () => JObject.Parse(expected).ToObject(typeof(ClientCapabilities), new Serializer(ClientVersion.Lsp3).JsonSerializer);
            a.Should().NotThrow();
        }
    }
}
