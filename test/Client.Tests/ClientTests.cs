using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using System;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Client.Dispatcher;
using OmniSharp.Extensions.LanguageServer.Client.Protocol;
using OmniSharp.Extensions.LanguageServer.Client.Utilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    /// <summary>
    ///     Tests for <see cref="LanguageClient"/>.
    /// </summary>
    public class ClientTests
        : PipeServerTestBase
    {
        /// <summary>
        ///     Create a new <see cref="LanguageClient"/> test suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        public ClientTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        /// <summary>
        ///     Get an absolute document path for use in tests.
        /// </summary>
        string AbsoluteDocumentPath => IsWindows ? @"C:\Foo.txt" : "/Foo.txt";

        /// <summary>
        ///     The <see cref="LanguageClient"/> under test.
        /// </summary>
        LanguageClient LanguageClient { get; set; }

        /// <summary>
        ///     The server-side dispatcher.
        /// </summary>
        LspDispatcher ServerDispatcher { get; } = new LspDispatcher(new Serializer(ClientVersion.Lsp3));

        /// <summary>
        ///     The server-side connection.
        /// </summary>
        LspConnection ServerConnection { get; set; }

        /// <summary>
        ///     Ensure that the language client can successfully request Hover information.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request hover info", Skip = "Periodic failures")]
        public async Task Hover_Success()
        {
            await Connect();

            const int line = 5;
            const int column = 5;
            var expectedHoverContent = new MarkedStringsOrMarkupContent("123", "456", "789");

            ServerDispatcher.HandleRequest<TextDocumentPositionParams, Hover>(DocumentNames.Hover, (request, cancellationToken) =>
            {
                Assert.NotNull(request.TextDocument);

                Assert.Equal(AbsoluteDocumentPath,
                    DocumentUri.GetFileSystemPath(request.TextDocument.Uri)
                );

                Assert.Equal(line, request.Position.Line);
                Assert.Equal(column, request.Position.Character);

                return Task.FromResult(new Hover
                {
                    Contents = expectedHoverContent,
                    Range = new Range
                    {
                        Start = request.Position,
                        End = request.Position
                    }
                });
            });

            var hover = await LanguageClient.TextDocument.Hover(AbsoluteDocumentPath, line, column);

            Assert.NotNull(hover.Range);
            Assert.NotNull(hover.Range.Start);
            Assert.NotNull(hover.Range.End);

            Assert.Equal(line, hover.Range.Start.Line);
            Assert.Equal(column, hover.Range.Start.Character);

            Assert.Equal(line, hover.Range.End.Line);
            Assert.Equal(column, hover.Range.End.Character);

            Assert.NotNull(hover.Contents);
            Assert.True(expectedHoverContent.HasMarkedStrings);
            Assert.Equal(expectedHoverContent.MarkedStrings
                .Select(markedString => markedString.Value),
                hover.Contents.MarkedStrings.Select(
                    markedString => markedString.Value
                )
            );
        }

        /// <summary>
        ///     Ensure that the language client can successfully request Completions.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request completions", Skip = "Periodic failures")]
        public async Task Completions_Success()
        {
            await Connect();

            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedCompletionItems = new CompletionItem[]
            {
                new CompletionItem
                {
                    Kind = CompletionItemKind.Class,
                    Label = "Class1",
                    TextEdit = new TextEdit
                    {
                        Range = new Range
                        {
                            Start = new Position
                            {
                                Line = line,
                                Character = column
                            },
                            End = new Position
                            {
                                Line = line,
                                Character = column
                            }
                        },
                        NewText = "Class1",
                    }
                }
            };

            ServerDispatcher.HandleRequest<TextDocumentPositionParams, CompletionList>(DocumentNames.Completion, (request, cancellationToken) =>
            {
                Assert.NotNull(request.TextDocument);

                Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                Assert.Equal(line, request.Position.Line);
                Assert.Equal(column, request.Position.Character);

                return Task.FromResult(new CompletionList(
                    expectedCompletionItems,
                    isIncomplete: true
                ));
            });

            var actualCompletions = await LanguageClient.TextDocument.Completions(AbsoluteDocumentPath, line, column);

            Assert.True(actualCompletions.IsIncomplete, "completions.IsIncomplete");
            Assert.NotNull(actualCompletions.Items);

            var actualCompletionItems = actualCompletions.Items.ToArray();
            Assert.Collection(actualCompletionItems, actualCompletionItem =>
            {
                var expectedCompletionItem = expectedCompletionItems[0];

                Assert.Equal(expectedCompletionItem.Kind, actualCompletionItem.Kind);
                Assert.Equal(expectedCompletionItem.Label, actualCompletionItem.Label);

                Assert.NotNull(actualCompletionItem.TextEdit);
                Assert.Equal(expectedCompletionItem.TextEdit.NewText, actualCompletionItem.TextEdit.NewText);

                Assert.NotNull(actualCompletionItem.TextEdit.Range);
                Assert.NotNull(actualCompletionItem.TextEdit.Range.Start);
                Assert.NotNull(actualCompletionItem.TextEdit.Range.End);
                Assert.Equal(expectedCompletionItem.TextEdit.Range.Start.Line, actualCompletionItem.TextEdit.Range.Start.Line);
                Assert.Equal(expectedCompletionItem.TextEdit.Range.Start.Character, actualCompletionItem.TextEdit.Range.Start.Character);
                Assert.Equal(expectedCompletionItem.TextEdit.Range.End.Line, actualCompletionItem.TextEdit.Range.End.Line);
                Assert.Equal(expectedCompletionItem.TextEdit.Range.End.Character, actualCompletionItem.TextEdit.Range.End.Character);
            });
        }

        /// <summary>
        ///     Ensure that the language client can successfully request SignatureHelp.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request signature help", Skip = "Periodic failures")]
        public async Task SignatureHelp_Success()
        {
            await Connect();

            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedSignatureHelp = new SignatureHelp {
                ActiveParameter = 0,
                ActiveSignature = 0,
                Signatures = new[] {
                    new SignatureInformation {
                        Documentation = new StringOrMarkupContent("test documentation"),
                        Label = "TestSignature",
                        Parameters = new[] {
                            new ParameterInformation {
                                Documentation = "test parameter documentation",
                                Label = "parameter label"
                            }
                        }
                    }
                }
            };

            ServerDispatcher.HandleRequest<TextDocumentPositionParams, SignatureHelp>(DocumentNames.SignatureHelp, (request, cancellationToken) => {
                Assert.NotNull(request.TextDocument);

                Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                Assert.Equal(line, request.Position.Line);
                Assert.Equal(column, request.Position.Character);

                return Task.FromResult(expectedSignatureHelp);
            });

            var actualSignatureHelp = await LanguageClient.TextDocument.SignatureHelp(AbsoluteDocumentPath, line, column);

            Assert.Equal(expectedSignatureHelp.ActiveParameter, actualSignatureHelp.ActiveParameter);
            Assert.Equal(expectedSignatureHelp.ActiveSignature, actualSignatureHelp.ActiveSignature);

            var actualSignatures = actualSignatureHelp.Signatures.ToArray();
            Assert.Collection(actualSignatures, actualSignature => {
                var expectedSignature = expectedSignatureHelp.Signatures.ToArray()[0];

                Assert.True(actualSignature.Documentation.HasString);
                Assert.Equal(expectedSignature.Documentation.String, actualSignature.Documentation.String);

                Assert.Equal(expectedSignature.Label, actualSignature.Label);

                var expectedParameters = expectedSignature.Parameters.ToArray();
                var actualParameters = actualSignature.Parameters.ToArray();

                Assert.Collection(actualParameters, actualParameter => {
                    var expectedParameter = expectedParameters[0];
                    Assert.True(actualParameter.Documentation.HasString);
                    Assert.Equal(expectedParameter.Documentation.String, actualParameter.Documentation.String);
                    Assert.Equal(expectedParameter.Label, actualParameter.Label);
                });
            });
        }

        /// <summary>
        ///     Ensure that the language client can successfully request Definition.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request definition", Skip = "Periodic failures")]
        public async Task Definition_Success()
        {
            await Connect();

            const int line = 5;
            const int column = 5;
            var expectedDocumentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(expectedDocumentPath);

            var expectedDefinitions = new LocationOrLocationLinks(
                new LocationOrLocationLink(new Location {
                    Uri = expectedDocumentUri,
                    Range = new Range {
                        Start = new Position {
                            Line = line,
                            Character = column
                        },
                        End = new Position {
                            Line = line,
                            Character = column
                        }
                    },
                }));

            ServerDispatcher.HandleRequest<TextDocumentPositionParams, LocationOrLocationLinks>(DocumentNames.Definition, (request, cancellationToken) => {
                Assert.NotNull(request.TextDocument);

                Assert.Equal(expectedDocumentUri, request.TextDocument.Uri);

                Assert.Equal(line, request.Position.Line);
                Assert.Equal(column, request.Position.Character);

                return Task.FromResult(expectedDefinitions);
            });

            var definitions = await LanguageClient.TextDocument.Definition(AbsoluteDocumentPath, line, column);

            var actualDefinitions = definitions.ToArray();
            Assert.Collection(actualDefinitions, actualDefinition => {
                var expectedDefinition = expectedDefinitions.First();

                Assert.NotNull(actualDefinition.Location);
                Assert.Equal(expectedDefinition.Location.Uri, actualDefinition.Location.Uri);

                Assert.NotNull(actualDefinition.Location.Range);
                Assert.NotNull(actualDefinition.Location.Range.Start);
                Assert.NotNull(actualDefinition.Location.Range.End);
                Assert.Equal(expectedDefinition.Location.Range.Start.Line, actualDefinition.Location.Range.Start.Line);
                Assert.Equal(expectedDefinition.Location.Range.Start.Character, actualDefinition.Location.Range.Start.Character);
                Assert.Equal(expectedDefinition.Location.Range.End.Line, actualDefinition.Location.Range.End.Line);
                Assert.Equal(expectedDefinition.Location.Range.End.Character, actualDefinition.Location.Range.End.Character);
            });
        }

        /// <summary>
        ///     Ensure that the language client can successfully receive Diagnostics from the server.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully receive diagnostics", Skip = "Periodic failures")]
        public async Task Diagnostics_Success()
        {
            await Connect();

            var documentPath = AbsoluteDocumentPath;
            var expectedDocumentUri = DocumentUri.FromFileSystemPath(documentPath);
            var expectedDiagnostics = new List<Diagnostic>
            {
                new Diagnostic
                {
                    Source = "Test",
                    Code = new DiagnosticCode(1234),
                    Message = "This is a diagnostic message.",
                    Range = new Range
                    {
                        Start = new Position
                        {
                            Line = 2,
                            Character = 5
                        },
                        End = new Position
                        {
                            Line = 3,
                            Character = 7
                        }
                    },
                    Severity = DiagnosticSeverity.Warning
                }
            };

            var receivedDiagnosticsNotification = new TaskCompletionSource<object>();

            Uri actualDocumentUri = null;
            List<Diagnostic> actualDiagnostics = null;
            LanguageClient.TextDocument.OnPublishDiagnostics((documentUri, diagnostics) =>
            {
                actualDocumentUri = documentUri;
                actualDiagnostics = diagnostics;

                receivedDiagnosticsNotification.SetResult(null);
            });

            ServerConnection.SendNotification(DocumentNames.PublishDiagnostics, new PublishDiagnosticsParams
            {
                Uri = DocumentUri.FromFileSystemPath(documentPath),
                Diagnostics = expectedDiagnostics
            });

            // Timeout.
            var winner = await Task.WhenAny(
                receivedDiagnosticsNotification.Task,
                Task.Delay(
                    TimeSpan.FromSeconds(2)
                )
            );
            Assert.Same(receivedDiagnosticsNotification.Task, winner);

            Assert.NotNull(actualDocumentUri);
            Assert.Equal(expectedDocumentUri, actualDocumentUri);

            Assert.NotNull(actualDiagnostics);
            Assert.Equal(1, actualDiagnostics.Count);

            var expectedDiagnostic = expectedDiagnostics[0];
            var actualDiagnostic = actualDiagnostics[0];

            Assert.Equal(expectedDiagnostic.Code, actualDiagnostic.Code);
            Assert.Equal(expectedDiagnostic.Message, actualDiagnostic.Message);
            Assert.Equal(expectedDiagnostic.Range.Start.Line, actualDiagnostic.Range.Start.Line);
            Assert.Equal(expectedDiagnostic.Range.Start.Character, actualDiagnostic.Range.Start.Character);
            Assert.Equal(expectedDiagnostic.Range.End.Line, actualDiagnostic.Range.End.Line);
            Assert.Equal(expectedDiagnostic.Range.End.Character, actualDiagnostic.Range.End.Character);
            Assert.Equal(expectedDiagnostic.Severity, actualDiagnostic.Severity);
            Assert.Equal(expectedDiagnostic.Source, actualDiagnostic.Source);
        }

        /// <summary>
        ///     Connect the client and server.
        /// </summary>
        /// <param name="handleServerInitialize">
        ///     Add standard handlers for server initialisation?
        /// </param>
        async Task Connect(bool handleServerInitialize = true)
        {
            ServerConnection = await CreateServerConnection();
            ServerConnection.Connect(ServerDispatcher);

            if (handleServerInitialize)
                HandleServerInitialize();

            LanguageClient = await CreateClient(initialize: true);
        }

        /// <summary>
        ///     Add standard handlers for sever initialisation.
        /// </summary>
        void HandleServerInitialize()
        {
            ServerDispatcher.HandleRequest<InitializeParams, InitializeResult>("initialize", (request, cancellationToken) =>
            {
                return Task.FromResult(new InitializeResult
                {
                    Capabilities = new ServerCapabilities
                    {
                        HoverProvider = true
                    }
                });
            });
            ServerDispatcher.HandleEmptyNotification("initialized", () =>
            {
                Log.LogInformation("Server initialized.");
            });
        }
    }
}
