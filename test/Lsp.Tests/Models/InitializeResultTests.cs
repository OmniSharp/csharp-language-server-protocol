using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Models
{
    public class InitializeResultTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InitializeResult()
            {
                Capabilities = new ServerCapabilities()
                {
                    CodeActionProvider = true,
                    CodeLensProvider = new CodeLensOptions()
                    {
                        ResolveProvider = true,
                    },
                    CompletionProvider = new CompletionOptions()
                    {
                        ResolveProvider = true,
                        TriggerCharacters = new[] { "a", "b", "c" }
                    },
                    DefinitionProvider = true,
                    DocumentFormattingProvider = true,
                    DocumentHighlightProvider = true,
                    DocumentLinkProvider = new DocumentLinkOptions()
                    {
                        ResolveProvider = true
                    },
                    DocumentOnTypeFormattingProvider = new DocumentOnTypeFormattingOptions()
                    {
                        FirstTriggerCharacter = ".",
                        MoreTriggerCharacter = new[] { ";", " " }
                    },
                    DocumentRangeFormattingProvider = true,
                    DocumentSymbolProvider = true,
                    ExecuteCommandProvider = new ExecuteCommandOptions()
                    {
                        Commands = new string[] { "command1", "command2" }
                    },
                    Experimental = new Dictionary<string, JToken>() {
                    { "abc", "123" }
                },
                    HoverProvider = true,
                    ReferencesProvider = true,
                    RenameProvider = true,
                    SignatureHelpProvider = new SignatureHelpOptions()
                    {
                        TriggerCharacters = new[] { ";", " " }
                    },
                    TextDocumentSync = new TextDocumentSync(new TextDocumentSyncOptions()
                    {
                        Change = TextDocumentSyncKind.Full,
                        OpenClose = true,
                        Save = new SaveOptions()
                        {
                            IncludeText = true
                        },
                        WillSave = true,
                        WillSaveWaitUntil = true
                    }),
                    WorkspaceSymbolProvider = true,
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<InitializeResult>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void BooleanOrTest(string expected)
        {
            var model = new InitializeResult()
            {
                Capabilities = new ServerCapabilities
                {
                    CodeActionProvider = new CodeActionOptions
                    {
                        CodeActionKinds = new[] {
                            CodeActionKind.QuickFix
                        }
                    },
                    ColorProvider = new DocumentColorOptions
                    {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.foo"),
                        Id = "foo"
                    },
                    DeclarationProvider = new DeclarationOptions
                    {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.foo"),
                        Id = "foo"
                    },
                    FoldingRangeProvider = new FoldingRangeOptions
                    {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.foo"),
                        Id = "foo"
                    },
                    ImplementationProvider = new ImplementationOptions
                    {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.foo"),
                        Id = "foo"
                    },
                    RenameProvider = new RenameOptions
                    {
                        PrepareProvider = true
                    },
                    TypeDefinitionProvider = new TypeDefinitionOptions
                    {
                        DocumentSelector = DocumentSelector.ForPattern("**/*.foo"),
                        Id = "foo"
                    }
                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<InitializeResult>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
