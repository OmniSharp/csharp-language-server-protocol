using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Capabilities.Client
{
    public class ClientCapabilitiesTests : AutoTestBase
    {
        // private const Fixtures =
        public ClientCapabilitiesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void ExtensionDataRoundTrips()
        {
            AssertExtensionDataRoundTrips(new LspSerializer());
        }

        [Fact]
        public void ExtensionDataRoundTripsWithProposals()
        {
            var serializer = new ProposedLspSerializer();
            var model = CreateClientCapabilitiesWithExtensionData();
            var initialize = new InitializeParams { Capabilities = model };

            var internalInitialize = serializer.DeserializeObject<InternalInitializeParams>(serializer.SerializeObject(initialize));
            var result = serializer.DeserializeObject<ClientCapabilities>(internalInitialize.Capabilities);

            result.Workspace!.ExtensionData.Should().ContainKey("unitTests");
        }

        private static void AssertExtensionDataRoundTrips(LspSerializer serializer)
        {
            var model = CreateClientCapabilitiesWithExtensionData();

            var result = serializer.DeserializeObject<ClientCapabilities>(serializer.SerializeObject(model));

            result.Workspace!.ExtensionData.Should().ContainKey("unitTests");
        }

        private static ClientCapabilities CreateClientCapabilitiesWithExtensionData() =>
            new ClientCapabilities {
                Workspace = new WorkspaceClientCapabilities {
                    ExtensionData = new Dictionary<string, JsonElement> {
                        ["unitTests"] = JsonSerializer.SerializeToElement(new { property = "Abcd" })
                    }
                }
            };

        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ClientCapabilities {
                Experimental = new Dictionary<string, JsonElement> {
                    { "abc", JsonSerializer.SerializeToElement("test") }
                },
                TextDocument = new TextDocumentClientCapabilities {
                    CodeAction = new CodeActionCapability { DynamicRegistration = true },
                    CodeLens = new CodeLensCapability { DynamicRegistration = true },
                    Definition = new DefinitionCapability { DynamicRegistration = true, LinkSupport = true },
                    Declaration = new DeclarationCapability { DynamicRegistration = true, LinkSupport = true },
                    DocumentHighlight = new DocumentHighlightCapability { DynamicRegistration = true },
                    DocumentLink = new DocumentLinkCapability { DynamicRegistration = true },
                    DocumentSymbol = new DocumentSymbolCapability { DynamicRegistration = true },
                    Formatting = new DocumentFormattingCapability { DynamicRegistration = true },
                    Hover = new HoverCapability { DynamicRegistration = true },
                    OnTypeFormatting = new DocumentOnTypeFormattingCapability { DynamicRegistration = true },
                    RangeFormatting = new DocumentRangeFormattingCapability { DynamicRegistration = true },
                    References = new ReferenceCapability { DynamicRegistration = true },
                    Rename = new RenameCapability { DynamicRegistration = true },
                    SignatureHelp = new SignatureHelpCapability { DynamicRegistration = true },
                    Completion = new CompletionCapability {
                        DynamicRegistration = true,
                        CompletionItem = new CompletionItemCapabilityOptions {
                            SnippetSupport = true
                        }
                    },
                    Implementation = new ImplementationCapability {
                        DynamicRegistration = true,
                        LinkSupport = true
                    },
                    TypeDefinition = new TypeDefinitionCapability {
                        DynamicRegistration = true,
                        LinkSupport = true
                    },
                    Synchronization = new TextSynchronizationCapability {
                        DynamicRegistration = true,
                        WillSave = true,
                        DidSave = true,
                        WillSaveWaitUntil = true
                    },
                    FoldingRange = new FoldingRangeCapability {
                        DynamicRegistration = true,
                        LineFoldingOnly = true,
                        RangeLimit = 5000
                    },
                    SelectionRange = new SelectionRangeCapability {
                        DynamicRegistration = true,
                        LineFoldingOnly = true,
                        RangeLimit = 5000
                    }
                },
                Workspace = new WorkspaceClientCapabilities {
                    ApplyEdit = true,
                    WorkspaceEdit = new WorkspaceEditCapability { DocumentChanges = true },
                    DidChangeConfiguration = new DidChangeConfigurationCapability { DynamicRegistration = true },
                    DidChangeWatchedFiles = new DidChangeWatchedFilesCapability { DynamicRegistration = true },
                    ExecuteCommand = new ExecuteCommandCapability { DynamicRegistration = true },
                    Symbol = new WorkspaceSymbolCapability { DynamicRegistration = true },
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<ClientCapabilities>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality().ConfigureForSupports(Logger).Excluding(z => z.Experimental)
            );
            deresult.Experimental["abc"].GetRawText().Should().Be(model.Experimental["abc"].GetRawText());
        }

        [Theory]
        [JsonFixture]
        public void Github_Issue_75(string expected)
        {
            Action a = () => new LspSerializer(ClientVersion.Lsp3).DeserializeObject<ClientCapabilities>(expected);
            a.Should().NotThrow();
        }
    }
}
