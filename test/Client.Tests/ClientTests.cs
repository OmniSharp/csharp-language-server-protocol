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
