using System;
using System.Collections.Generic;
using Autofac;
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
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
                    CodeAction = new CodeActionClientCapabilities() { DynamicRegistration = true },
                    CodeLens = new CodeLensClientCapabilities() { DynamicRegistration = true },
                    Definition = new DefinitionClientCapabilities() { DynamicRegistration = true, LinkSupport = true },
                    Declaration = new DeclarationClientCapabilities() { DynamicRegistration = true, LinkSupport = true },
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
                    Implementation = new ImplementationClientCapabilities()
                    {
                        DynamicRegistration = true,
                        LinkSupport = true
                    },
                    TypeDefinition = new TypeDefinitionClientCapabilities()
                    {
                        DynamicRegistration = true,
                        LinkSupport = true
                    },
                    Synchronization = new TextDocumentSyncClientCapabilities()
                    {
                        DynamicRegistration = true,
                        WillSave = true,
                        DidSave = true,
                        WillSaveWaitUntil = true
                    },
                    FoldingRange = new FoldingRangeClientCapabilities()
                    {
                        DynamicRegistration = true,
                        LineFoldingOnly = true,
                        RangeLimit = 5000
                    }

                },
                Workspace = new WorkspaceClientCapabilities()
                {
                    ApplyEdit = true,
                    WorkspaceEdit = new WorkspaceEditClientCapabilities() { DocumentChanges = true },
                    DidChangeConfiguration = new DidChangeConfigurationClientCapabilities() { DynamicRegistration = true },
                    DidChangeWatchedFiles = new DidChangeWatchedFilesClientCapabilities() { DynamicRegistration = true },
                    ExecuteCommand = new ExecuteCommandClientCapabilities() { DynamicRegistration = true },
                    Symbol = new WorkspaceSymbolClientCapabilities() { DynamicRegistration = true },
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
