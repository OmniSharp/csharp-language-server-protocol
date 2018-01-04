using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client.Clients;
using OmniSharp.Extensions.LanguageServer.Client.Dispatcher;
using OmniSharp.Extensions.LanguageServer.Client.Handlers;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Client.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     A client for the Language Server Protocol.
    /// </summary>
    /// <remarks>
    ///     Note - at this stage, a <see cref="LanguageClient"/> cannot be reused once <see cref="Shutdown"/> has been called; instead, create a new one.
    /// </remarks>
    public sealed class LanguageClient
        : IDisposable
    {
        /// <summary>
        ///     The serialiser for notification / request / response bodies.
        /// </summary>
        /// <remarks>
        ///     TODO: Make this injectable. And what does client version do - do we have to negotiate this?
        /// </remarks>
        readonly ISerializer _serializer = new Serializer(ClientVersion.Lsp3);

        /// <summary>
        ///     The dispatcher for incoming requests, notifications, and responses.
        /// </summary>
        readonly LspDispatcher _dispatcher;

        /// <summary>
        ///     The handler for dynamic registration of server capabilities.
        /// </summary>
        /// <remarks>
        ///     We don't actually support this yet but some server implementations (e.g. OmniSharp) will freak out if we don't respond to the message, even if we've indicated that we don't support dynamic registrations of server capabilities.
        /// </remarks>
        readonly DynamicRegistrationHandler _dynamicRegistrationHandler = new DynamicRegistrationHandler();

        /// <summary>
        ///     The language server process.
        /// </summary>
        ServerProcess _process;

        /// <summary>
        ///     The underlying LSP connection to the language server process.
        /// </summary>
        LspConnection _connection;

        /// <summary>
        ///     Completion source that callers can await to determine when the language server is ready to use (i.e. initialised).
        /// </summary>
        TaskCompletionSource<object> _readyCompletion = new TaskCompletionSource<object>();

        /// <summary>
        ///     Create a new <see cref="LanguageClient"/>.
        /// </summary>
        /// <param name="loggerFactory">
        ///     The factory for loggers used by the client and its components.
        /// </param>
        /// <param name="serverStartInfo">
        ///     A <see cref="ProcessStartInfo"/> describing how to start the server process.
        /// </param>
        public LanguageClient(ILoggerFactory loggerFactory, ProcessStartInfo serverStartInfo)
            : this(loggerFactory, new StdioServerProcess(loggerFactory, serverStartInfo))
        {
        }

        /// <summary>
        ///     Create a new <see cref="LanguageClient"/>.
        /// </summary>
        /// <param name="loggerFactory">
        ///     The factory for loggers used by the client and its components.
        /// </param>
        /// <param name="process">
        ///     A <see cref="ServerProcess"/> used to start or connect to the server process.
        /// </param>
        public LanguageClient(ILoggerFactory loggerFactory, ServerProcess process)
            : this(loggerFactory)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            _process = process;
            _process.Exited += ServerProcess_Exit;
        }

        /// <summary>
        ///     Create a new <see cref="LanguageClient"/>.
        /// </summary>
        /// <param name="loggerFactory">
        ///     The logger to use.
        /// </param>
        LanguageClient(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            LoggerFactory = loggerFactory;
            Log = LoggerFactory.CreateLogger<LanguageClient>();
            Workspace = new WorkspaceClient(this);
            Window = new WindowClient(this);
            TextDocument = new TextDocumentClient(this);

            _dispatcher = new LspDispatcher(_serializer);
            _dispatcher.RegisterHandler(_dynamicRegistrationHandler);
        }

        /// <summary>
        ///     Dispose of resources being used by the client.
        /// </summary>
        public void Dispose()
        {
            var connection = Interlocked.Exchange(ref _connection, null);
            connection?.Dispose();

            var serverProcess = Interlocked.Exchange(ref _process, null);
            serverProcess?.Dispose();
        }

        /// <summary>
        ///     The factory for loggers used by the client and its components.
        /// </summary>
        ILoggerFactory LoggerFactory { get; }

        /// <summary>
        ///     The client's logger.
        /// </summary>
        ILogger Log { get; }

        /// <summary>
        ///     The LSP Text Document API.
        /// </summary>
        public TextDocumentClient TextDocument { get; }

        /// <summary>
        ///     The LSP Window API.
        /// </summary>
        public WindowClient Window { get; }

        /// <summary>
        ///     The LSP Workspace API.
        /// </summary>
        public WorkspaceClient Workspace { get; }

        /// <summary>
        ///     The client's capabilities.
        /// </summary>
        public ClientCapabilities ClientCapabilities { get; } = new ClientCapabilities
        {
            Workspace = new WorkspaceClientCapabilites
            {
                DidChangeConfiguration = new DidChangeConfigurationCapability
                {
                    DynamicRegistration = false
                }
            },
            TextDocument = new TextDocumentClientCapabilities
            {
                Synchronization = new SynchronizationCapability
                {
                    DidSave = true,
                    DynamicRegistration = false
                },
                Hover = new HoverCapability
                {
                    DynamicRegistration = false
                },
                Completion = new CompletionCapability
                {
                    CompletionItem = new CompletionItemCapability
                    {
                        SnippetSupport = false
                    },
                    DynamicRegistration = false
                }
            }
        };

        /// <summary>
        ///     The server's capabilities.
        /// </summary>
        public ServerCapabilities ServerCapabilities => _dynamicRegistrationHandler.ServerCapabilities;

        /// <summary>
        ///     Has the language client been initialised?
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///     Is the connection to the language server open?
        /// </summary>
        public bool IsConnected => _connection != null && _connection.IsOpen;

        /// <summary>
        ///     A <see cref="Task"/> that completes when the client is ready to handle requests.
        /// </summary>
        public Task IsReady => _readyCompletion.Task;

        /// <summary>
        ///     A <see cref="Task"/> that completes when the underlying connection has closed and the server has stopped.
        /// </summary>
        public Task HasShutdown
        {
            get
            {
                return Task.WhenAll(
                    _connection.HasHasDisconnected,
                    _process?.HasExited ?? Task.CompletedTask
                );
            }
        }

        /// <summary>
        ///     Initialise the language server.
        /// </summary>
        /// <param name="workspaceRoot">
        ///     The workspace root.
        /// </param>
        /// <param name="initializationOptions">
        ///     An optional <see cref="object"/> representing additional options to send to the server.
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing initialisation.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Initialize(string, object, CancellationToken)"/> has already been called.
        ///
        ///     <see cref="Initialize(string, object, CancellationToken)"/> can only be called once per <see cref="LanguageClient"/>; if you have called <see cref="Shutdown"/>, you will need to use a new <see cref="LanguageClient"/>.
        /// </exception>
        public async Task Initialize(string workspaceRoot, object initializationOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsInitialized)
                throw new InvalidOperationException("Client has already been initialised.");

            try
            {
                await Start();

                var initializeParams = new InitializeParams
                {
                    RootPath = workspaceRoot,
                    Capabilities = ClientCapabilities,
                    ProcessId = Process.GetCurrentProcess().Id,
                    InitializationOptions = initializationOptions
                };

                Log.LogDebug("Sending 'initialize' message to language server...");

                var result = await SendRequest<InitializeResult>("initialize", initializeParams, cancellationToken).ConfigureAwait(false);
                if (result == null)
                    throw new LspException("Server replied to 'initialize' request with a null response.");

                _dynamicRegistrationHandler.ServerCapabilities = result.Capabilities;

                Log.LogDebug("Sent 'initialize' message to language server.");

                Log.LogDebug("Sending 'initialized' notification to language server...");

                SendNotification("initialized");

                Log.LogDebug("Sent 'initialized' notification to language server.");

                IsInitialized = true;
                _readyCompletion.TrySetResult(null);
            }
            catch (Exception initializationError)
            {
                // Capture the initialisation error so anyone awaiting IsReady will also see it.
                _readyCompletion.TrySetException(initializationError);

                throw;
            }
        }

        /// <summary>
        ///     Stop the language server.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the shutdown operation.
        /// </returns>
        public async Task Shutdown()
        {
            var connection = _connection;
            if (connection != null)
            {
                if (connection.IsOpen)
                {
                    connection.SendEmptyNotification("shutdown");
                    connection.SendEmptyNotification("exit");
                    connection.Disconnect(flushOutgoing: true);
                }

                await connection.HasHasDisconnected;
            }

            var serverProcess = _process;
            if (serverProcess != null)
            {
                if (serverProcess.IsRunning)
                    await serverProcess.Stop();
            }

            IsInitialized = false;
            _readyCompletion = new TaskCompletionSource<object>();
        }

        /// <summary>
        ///     Register a message handler.
        /// </summary>
        /// <param name="handler">
        ///     The message handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public IDisposable RegisterHandler(IHandler handler) => _dispatcher.RegisterHandler(handler);

        /// <summary>
        ///     Send an empty notification to the language server.
        /// </summary>
        /// <param name="method">
        ///     The notification method name.
        /// </param>
        public void SendNotification(string method)
        {
            var connection = _connection;
            if (connection == null || !connection.IsOpen)
                throw new InvalidOperationException("Not connected to the language server.");

            connection.SendEmptyNotification(method);
        }

        /// <summary>
        ///     Send a notification message to the language server.
        /// </summary>
        /// <param name="method">
        ///     The notification method name.
        /// </param>
        /// <param name="notification">
        ///     The notification message.
        /// </param>
        public void SendNotification(string method, object notification)
        {
            var connection = _connection;
            if (connection == null || !connection.IsOpen)
                throw new InvalidOperationException("Not connected to the language server.");

            connection.SendNotification(method, notification);
        }

        /// <summary>
        ///     Send a request to the language server.
        /// </summary>
        /// <param name="method">
        ///     The request method name.
        /// </param>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional cancellation token that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the request.
        /// </returns>
        public Task SendRequest(string method, object request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = _connection;
            if (connection == null || !connection.IsOpen)
                throw new InvalidOperationException("Not connected to the language server.");

            return connection.SendRequest(method, request, cancellationToken);
        }

        /// <summary>
        ///     Send a request to the language server.
        /// </summary>
        /// <typeparam name="TResponse">
        ///     The response message type.
        /// </typeparam>
        /// <param name="method">
        ///     The request method name.
        /// </param>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellation">
        ///     An optional cancellation token that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> representing the response.
        /// </returns>
        public Task<TResponse> SendRequest<TResponse>(string method, object request, CancellationToken cancellation = default(CancellationToken))
        {
            var connection = _connection;
            if (connection == null || !connection.IsOpen)
                throw new InvalidOperationException("Not connected to the language server.");

            return connection.SendRequest<TResponse>(method, request, cancellation);
        }

        /// <summary>
        ///     Start the language server.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        async Task Start()
        {
            if (_process == null)
                throw new ObjectDisposedException(GetType().Name);

            if (!_process.IsRunning)
            {
                Log.LogDebug("Starting language server...");

                await _process.Start();

                Log.LogDebug("Language server is running.");
            }

            Log.LogDebug("Opening connection to language server...");

            if (_connection == null)
                _connection = new LspConnection(LoggerFactory, input: _process.OutputStream, output: _process.InputStream);

            _connection.Connect(_dispatcher);

            Log.LogDebug("Connection to language server is open.");
        }

        /// <summary>
        ///     Called when the server process has exited.
        /// </summary>
        /// <param name="sender">
        ///     The event sender.
        /// </param>
        /// <param name="args">
        ///     The event arguments.
        /// </param>
        async void ServerProcess_Exit(object sender, EventArgs args)
        {
            Log.LogDebug("Server process has exited; language client is shutting down...");

            var connection = Interlocked.Exchange(ref _connection, null);
            if (connection != null)
            {
                using (connection)
                {
                    connection.Disconnect();
                    await connection.HasHasDisconnected;
                }
            }

            await Shutdown();

            Log.LogDebug("Language client shutdown complete.");
        }
    }
}
