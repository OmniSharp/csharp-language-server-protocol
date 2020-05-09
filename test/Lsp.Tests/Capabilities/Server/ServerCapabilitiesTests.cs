using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Xunit;

namespace Lsp.Tests.Capabilities.Server
{
    public class ServerCapabilitiesTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ServerCapabilities()
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
                ColorProvider = true,
                FoldingRangeProvider = true,
                ImplementationProvider = true,
                TypeDefinitionProvider = true
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<ServerCapabilities>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void Optional(string expected)
        {
            var model = new ServerCapabilities
            {
                ColorProvider = (DocumentColorOptions)null
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<ServerCapabilities>(expected);
            deresult.Should().BeEquivalentTo(model);
        }
    }
}
