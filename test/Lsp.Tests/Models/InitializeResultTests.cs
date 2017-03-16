using System;
using System.Collections.Generic;
using FluentAssertions;
using Lsp.Capabilities.Server;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class InitializeResultTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new InitializeResult() {
                Capabilities = new ServerCapabilities() {
                    CodeActionProvider = true,
                    CodeLensProvider = new CodeLensOptions() {
                        ResolveProvider = true,
                    },
                    CompletionProvider = new CompletionOptions() {
                        ResolveProvider = true,
                        TriggerCharacters = new[] { "a", "b", "c" }
                    },
                    DefinitionProvider = true,
                    DocumentFormattingProvider = true,
                    DocumentHighlightProvider = true,
                    DocumentLinkProvider = new DocumentLinkOptions() {
                        ResolveProvider = true
                    },
                    DocumentOnTypeFormattingProvider = new DocumentOnTypeFormattingOptions() {
                        FirstTriggerCharacter = ".",
                        MoreTriggerCharacter = new[] { ";", " " }
                    },
                    DocumentRangeFormattingProvider = true,
                    DocumentSymbolProvider = true,
                    ExecuteCommandProvider = new ExecuteCommandOptions() {
                        Commands = new string[] { "command1", "command2" }
                    },
                    Experimental = new Dictionary<string, object>() {
                    { "abc", "123" }
                },
                    HoverProvider = true,
                    ReferencesProvider = true,
                    RenameProvider = true,
                    SignatureHelpProvider = new SignatureHelpOptions() {
                        TriggerCharacters = new[] { ";", " " }
                    },
                    TextDocumentSync = new TextDocumentSync(new TextDocumentSyncOptions() {
                        Change = TextDocumentSyncKind.Full,
                        OpenClose = true,
                        Save = new SaveOptions() {
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

            var deresult = JsonConvert.DeserializeObject<InitializeResult>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
