using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using TestingUtils;

namespace Lsp.Tests.Models
{
    public class InitializeResultTests
    {
        [Theory]
        [JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InitializeResult {
                Capabilities = new ServerCapabilities {
                    CodeActionProvider = true,
                    CodeLensProvider = new CodeLensRegistrationOptions.StaticOptions {
                        ResolveProvider = true,
                    },
                    CompletionProvider = new CompletionRegistrationOptions.StaticOptions {
                        ResolveProvider = true,
                        TriggerCharacters = new[] { "a", "b", "c" }
                    },
                    DefinitionProvider = true,
                    DocumentFormattingProvider = true,
                    DocumentHighlightProvider = true,
                    DocumentLinkProvider = new DocumentLinkRegistrationOptions.StaticOptions {
                        ResolveProvider = true
                    },
                    DocumentOnTypeFormattingProvider = new DocumentOnTypeFormattingRegistrationOptions.StaticOptions {
                        FirstTriggerCharacter = ".",
                        MoreTriggerCharacter = new[] { ";", " " }
                    },
                    DocumentRangeFormattingProvider = true,
                    DocumentSymbolProvider = true,
                    ExecuteCommandProvider = new ExecuteCommandRegistrationOptions.StaticOptions {
                        Commands = new[] { "command1", "command2" }
                    },
                    Experimental = new Dictionary<string, JToken> {
                        { "abc", "123" }
                    },
                    HoverProvider = true,
                    ReferencesProvider = true,
                    RenameProvider = true,
                    SignatureHelpProvider = new SignatureHelpRegistrationOptions.StaticOptions {
                        TriggerCharacters = new[] { ";", " " }
                    },
                    TextDocumentSync = new TextDocumentSync(
                        new TextDocumentSyncOptions {
                            Change = TextDocumentSyncKind.Full,
                            OpenClose = true,
                            Save = new SaveOptions {
                                IncludeText = true
                            },
                            WillSave = true,
                            WillSaveWaitUntil = true
                        }
                    ),
                    WorkspaceSymbolProvider = true,
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<InitializeResult>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }

        [Theory]
        [JsonFixture]
        public void BooleanOrTest(string expected)
        {
            var model = new InitializeResult {
                Capabilities = new ServerCapabilities {
                    CodeActionProvider = new CodeActionRegistrationOptions.StaticOptions() {
                        CodeActionKinds = new[] {
                            CodeActionKind.QuickFix
                        }
                    },
                    ColorProvider = new DocumentColorRegistrationOptions.StaticOptions {
                        Id = "foo"
                    },
                    DeclarationProvider = new DeclarationRegistrationOptions.StaticOptions {
                        Id = "foo"
                    },
                    FoldingRangeProvider = new FoldingRangeRegistrationOptions.StaticOptions {
                        Id = "foo"
                    },
                    ImplementationProvider = new ImplementationRegistrationOptions.StaticOptions {
                        Id = "foo"
                    },
                    RenameProvider = new RenameRegistrationOptions.StaticOptions {
                        PrepareProvider = true
                    },
                    TypeDefinitionProvider = new TypeDefinitionRegistrationOptions.StaticOptions {
                        Id = "foo"
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new LspSerializer(ClientVersion.Lsp3).DeserializeObject<InitializeResult>(expected);
            deresult.Should().BeEquivalentTo(model, x => x.UsingStructuralRecordEquality());
        }
    }
}
