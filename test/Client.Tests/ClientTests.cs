using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServerProtocol.Client.Dispatcher;
using OmniSharp.Extensions.LanguageServerProtocol.Client.Protocol;
using OmniSharp.Extensions.LanguageServerProtocol.Client.Utilities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using System;

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
        LspDispatcher ServerDispatcher { get; } = new LspDispatcher();

        /// <summary>
        ///     The server-side connection.
        /// </summary>
        LspConnection ServerConnection { get; set; }

        /// <summary>
        ///     Ensure that the language client can successfully request Hover information.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully request hover info")]
        public async Task Hover_Success()
        {
            await Connect();

            const int line = 5;
            const int column = 5;
            var expectedHoverContent = new MarkedStringContainer("123", "456", "789");

            ServerDispatcher.HandleRequest<TextDocumentPositionParams, Hover>("textDocument/hover", (request, cancellationToken) =>
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

            Hover hover = await LanguageClient.TextDocument.Hover(AbsoluteDocumentPath, line, column);

            Assert.NotNull(hover.Range);
            Assert.NotNull(hover.Range.Start);
            Assert.NotNull(hover.Range.End);

            Assert.Equal(line, hover.Range.Start.Line);
            Assert.Equal(column, hover.Range.Start.Character);

            Assert.Equal(line, hover.Range.End.Line);
            Assert.Equal(column, hover.Range.End.Character);

            Assert.NotNull(hover.Contents);
            Assert.Equal(expectedHoverContent.Select(markedString => markedString.Value),
                hover.Contents.Select(
                    markedString => markedString.Value
                )
            );
        }

        /// <summary>
        ///     Ensure that the language client can successfully receive Diagnostics from the server.
        /// </summary>
        [Fact(DisplayName = "Language client can successfully receive diagnostics")]
        public async Task Diagnostics_Success()
        {
            await Connect();

            string documentPath = AbsoluteDocumentPath;
            Uri expectedDocumentUri = DocumentUri.FromFileSystemPath(documentPath);
            List<Diagnostic> expectedDiagnostics = new List<Diagnostic>
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

            TaskCompletionSource<object> receivedDiagnosticsNotification = new TaskCompletionSource<object>();

            Uri actualDocumentUri = null;
            List<Diagnostic> actualDiagnostics = null;
            LanguageClient.TextDocument.OnPublishDiagnostics((documentUri, diagnostics) =>
            {
                actualDocumentUri = documentUri;
                actualDiagnostics = diagnostics;

                receivedDiagnosticsNotification.SetResult(null);
            });

            ServerConnection.SendNotification("textDocument/publishDiagnostics", new PublishDiagnosticsParams
            {
                Uri = DocumentUri.FromFileSystemPath(documentPath),
                Diagnostics = expectedDiagnostics
            });

            // Timeout.
            Task winner = await Task.WhenAny(
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

            Diagnostic expectedDiagnostic = expectedDiagnostics[0];
            Diagnostic actualDiagnostic = actualDiagnostics[0];

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
