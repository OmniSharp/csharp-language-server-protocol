using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Handlers;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServer : ILanguageServer, IInitializeHandler, IInitializedHandler, IAwaitableTermination
    {
        private readonly Connection _connection;
        private readonly ILspRequestRouter _requestRouter;
        private readonly ShutdownHandler _shutdownHandler = new ShutdownHandler();
        private readonly ExitHandler _exitHandler;
        private ClientVersion? _clientVersion;
        private readonly ILspReciever _reciever;
        private readonly ISerializer _serializer;
        private readonly IHandlerCollection _collection;
        private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IResponseRouter _responseRouter;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IServiceProvider _serviceProvider;
        private SupportedCapabilities _supportedCapabilities;

        public static Task<ILanguageServer> From(Action<LanguageServerOptions> optionsAction)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return From(options);
        }

        public static async Task<ILanguageServer> From(LanguageServerOptions options)
        {
            var server = new LanguageServer(
                options.Input,
                options.Output,
                options.Reciever,
                options.RequestProcessIdentifier,
                options.LoggerFactory,
                options.Serializer,
                options.Services,
                options.HandlerTypes.Select(x => x.Assembly)
                    .Distinct().Concat(options.HandlerAssemblies),
                options.InitializeDelegates,
                options.InitializedDelegates
            );

            if (options.AddDefaultLoggingProvider)
                options.LoggerFactory.AddProvider(new LanguageServerLoggerProvider(server));

            await server.Initialize();

            return server;
        }

        internal LanguageServer(
            Stream input,
            Stream output,
            ILspReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            ILoggerFactory loggerFactory,
            ISerializer serializer,
            IServiceCollection services,
            IEnumerable<Assembly> assemblies,
            IEnumerable<InitializeDelegate> initializeDelegates,
            IEnumerable<InitializedDelegate> initializedDelegates)
        {
            var outputHandler = new OutputHandler(output, serializer);

            services.AddLogging();
            _reciever = reciever;
            _serializer = serializer;
            _supportedCapabilities = new SupportedCapabilities();
            _collection = new HandlerCollection(_supportedCapabilities);
            _initializeDelegates = initializeDelegates;
            _initializedDelegates = initializedDelegates;

            services.AddSingleton<IOutputHandler>(outputHandler);
            services.AddSingleton(_collection);
            services.AddSingleton(_serializer);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(_serializer);
            services.AddSingleton(requestProcessIdentifier);
            services.AddSingleton<IReceiver>(_reciever);
            services.AddSingleton<ILspReciever>(_reciever);
            services.AddSingleton(loggerFactory);

            services.AddJsonRpcMediatR(assemblies);
            services.AddTransient<IHandlerMatcher, TextDocumentMatcher>();
            services.AddSingleton<Protocol.Server.ILanguageServer>(this);
            services.AddSingleton<ILanguageServer>(this);
            services.AddTransient<IHandlerMatcher, ExecuteCommandMatcher>();
            services.AddTransient<IHandlerMatcher, ResolveCommandMatcher>();
            services.AddSingleton<ILspRequestRouter, LspRequestRouter>();
            services.AddSingleton<IRequestRouter>(_ => _.GetRequiredService<ILspRequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResolveCommandPipeline<,>));

            var foundHandlers = services
                .Where(x => typeof(IJsonRpcHandler).IsAssignableFrom(x.ServiceType) && x.ServiceType != typeof(IJsonRpcHandler))
                .ToArray();

            // Handlers are created at the start and maintained as a singleton
            foreach (var handler in foundHandlers)
            {
                services.Remove(handler);

                if (handler.ImplementationFactory != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationFactory));
                else if (handler.ImplementationInstance != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationInstance));
                else
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationType));
            }

            _serviceProvider = services.BuildServiceProvider();

            _requestRouter = _serviceProvider.GetRequiredService<ILspRequestRouter>();
            _responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _connection = ActivatorUtilities.CreateInstance<Connection>(_serviceProvider, input);

            _exitHandler = new ExitHandler(_shutdownHandler);

            _disposable.Add(
                AddHandlers(this, _shutdownHandler, _exitHandler, new CancelRequestHandler(_requestRouter))
            );

            var handlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
            _collection.Add(handlers);

            Document = new LanguageServerDocument(_responseRouter);
            Client = new LanguageServerClient(_responseRouter);
            Window = new LanguageServerWindow(_responseRouter);
            Workspace = new LanguageServerWorkspace(_responseRouter);
        }

        public ILanguageServerDocument Document { get; }
        public ILanguageServerClient Client { get; }
        public ILanguageServerWindow Window { get; }
        public ILanguageServerWorkspace Workspace { get; }

        public InitializeParams ClientSettings { get; private set; }
        public InitializeResult ServerSettings { get; private set; }

        /// <summary>
        ///     The minimum level for the server's default logger.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            var handlerDisposable = _collection.Add(method, handler);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            var handlerDisposable = _collection.Add(handlers);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandler(string method, Type handlerType)
        {
            var handlerDisposable = _collection.Add(method, _serviceProvider, handlerType);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandler<T>()
            where T : IJsonRpcHandler
        {
            return AddHandlers(typeof(T));
        }

        public IDisposable AddHandlers(params Type[] handlerTypes)
        {
            var handlerDisposable = _collection.Add(_serviceProvider, handlerTypes);
            return RegisterHandlers(handlerDisposable);
        }

        private IDisposable RegisterHandlers(LspHandlerDescriptorDisposable handlerDisposable)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var registrations = handlerDisposable.Descriptors
                    .Select(x => x.Registration)
                    .Where(x => x != null)
                    .ToArray();

                // Fire and forget
                DynamicallyRegisterHandlers(registrations).ToObservable().Subscribe();

                return new ImmutableDisposable(
                    handlerDisposable,
                    new Disposable(() =>
                    {
                        Client.UnregisterCapability(new UnregistrationParams()
                        {
                            Unregisterations = registrations.ToArray()
                        }).ToObservable().Subscribe();
                    }));
            }
        }

        private async Task Initialize()
        {
            _connection.Open();

            await _initializeComplete;
        }

        async Task<InitializeResult> IRequestHandler<InitializeParams, InitializeResult>.Handle(InitializeParams request, CancellationToken token)
        {
            ClientSettings = request;

            if (request.Trace == InitializeTrace.Verbose && MinimumLogLevel >= LogLevel.Information)
            {
                MinimumLogLevel = LogLevel.Trace;
            }

            await Task.WhenAll(_initializeDelegates.Select(c => c(request)));

            _clientVersion = request.Capabilities.GetClientVersion();
            _serializer.SetClientCapabilities(_clientVersion.Value, request.Capabilities);

            var supportedCapabilites = new List<ISupports>();
            if (_clientVersion == ClientVersion.Lsp3)
            {
                if (request.Capabilities.TextDocument != null)
                {
                    supportedCapabilites.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(request.Capabilities.TextDocument)
                    );
                }

                if (request.Capabilities.Workspace != null)
                {
                    supportedCapabilites.AddRange(
                        LspHandlerDescriptorHelpers.GetSupportedCapabilities(request.Capabilities.Workspace)
                    );
                }
            }

            _supportedCapabilities.Add(supportedCapabilites);

            AddHandlers(_serviceProvider.GetServices<IJsonRpcHandler>().ToArray());

            var textDocumentCapabilities = ClientSettings.Capabilities.TextDocument;
            var workspaceCapabilities = ClientSettings.Capabilities.Workspace;

            var ccp = new ClientCapabilityProvider(_collection);

            var serverCapabilities = new ServerCapabilities()
            {
                CodeActionProvider = ccp.HasStaticHandler(textDocumentCapabilities.CodeAction),
                CodeLensProvider = ccp.GetStaticOptions(textDocumentCapabilities.CodeLens).Get<ICodeLensOptions, CodeLensOptions>(CodeLensOptions.Of),
                CompletionProvider = ccp.GetStaticOptions(textDocumentCapabilities.Completion).Get<ICompletionOptions, CompletionOptions>(CompletionOptions.Of),
                DefinitionProvider = ccp.HasStaticHandler(textDocumentCapabilities.Definition),
                DocumentFormattingProvider = ccp.HasStaticHandler(textDocumentCapabilities.Formatting),
                DocumentHighlightProvider = ccp.HasStaticHandler(textDocumentCapabilities.DocumentHighlight),
                DocumentLinkProvider = ccp.GetStaticOptions(textDocumentCapabilities.DocumentLink).Get<IDocumentLinkOptions, DocumentLinkOptions>(DocumentLinkOptions.Of),
                DocumentOnTypeFormattingProvider = ccp.GetStaticOptions(textDocumentCapabilities.OnTypeFormatting).Get<IDocumentOnTypeFormattingOptions, DocumentOnTypeFormattingOptions>(DocumentOnTypeFormattingOptions.Of),
                DocumentRangeFormattingProvider = ccp.HasStaticHandler(textDocumentCapabilities.RangeFormatting),
                DocumentSymbolProvider = ccp.HasStaticHandler(textDocumentCapabilities.DocumentSymbol),
                ExecuteCommandProvider = ccp.GetStaticOptions(workspaceCapabilities.ExecuteCommand).Reduce<IExecuteCommandOptions, ExecuteCommandOptions>(ExecuteCommandOptions.Of),
                HoverProvider = ccp.HasStaticHandler(textDocumentCapabilities.Hover),
                ReferencesProvider = ccp.HasStaticHandler(textDocumentCapabilities.References),
                RenameProvider = ccp.HasStaticHandler(textDocumentCapabilities.Rename),
                SignatureHelpProvider = ccp.GetStaticOptions(textDocumentCapabilities.SignatureHelp).Get<ISignatureHelpOptions, SignatureHelpOptions>(SignatureHelpOptions.Of),
                WorkspaceSymbolProvider = ccp.HasStaticHandler(workspaceCapabilities.Symbol),
                ImplementationProvider = ccp.GetStaticOptions(textDocumentCapabilities.Implementation).Get<IImplementationOptions, ImplementationOptions>(ImplementationOptions.Of),
                TypeDefinitionProvider = ccp.GetStaticOptions(textDocumentCapabilities.TypeDefinition).Get<ITypeDefinitionOptions, TypeDefinitionOptions>(TypeDefinitionOptions.Of),
                ColorProvider = ccp.GetStaticOptions(textDocumentCapabilities.ColorProvider).Get<IColorOptions, StaticColorOptions>(ColorOptions.Of),
            };

            if (_collection.ContainsHandler(typeof(IDidChangeWorkspaceFoldersHandler)))
            {
                serverCapabilities.Workspace = new WorkspaceServerCapabilities()
                {
                    WorkspaceFolders = new WorkspaceFolderOptions()
                    {
                        Supported = true,
                        ChangeNotifications = Guid.NewGuid().ToString()
                    }
                };
            }

            var textDocumentSyncKind = _collection.ContainsHandler(typeof(IDidChangeTextDocumentHandler))
                ? _collection
                    .Select(x => x.Handler)
                    .OfType<IDidChangeTextDocumentHandler>()
                    .Where(x => x.Change != TextDocumentSyncKind.None)
                    .Min(z => z.Change)
                : TextDocumentSyncKind.None;

            if (_clientVersion == ClientVersion.Lsp2)
            {
                serverCapabilities.TextDocumentSync = textDocumentSyncKind;
            }
            else
            {
                serverCapabilities.TextDocumentSync = new TextDocumentSyncOptions()
                {
                    Change = textDocumentSyncKind,
                    OpenClose = _collection.ContainsHandler(typeof(IDidOpenTextDocumentHandler)) || _collection.ContainsHandler(typeof(IDidCloseTextDocumentHandler)),
                    Save = _collection.ContainsHandler(typeof(IDidSaveTextDocumentHandler)) ?
                        new SaveOptions() { IncludeText = true /* TODO: Make configurable */ } :
                        null,
                    WillSave = _collection.ContainsHandler(typeof(IWillSaveTextDocumentHandler)),
                    WillSaveWaitUntil = _collection.ContainsHandler(typeof(IWillSaveWaitUntilTextDocumentHandler))
                };
            }

            // TODO: Need a call back here
            // serverCapabilities.Experimental;

            _reciever.Initialized();

            var result = ServerSettings = new InitializeResult() { Capabilities = serverCapabilities };

            await Task.WhenAll(_initializedDelegates.Select(c => c(request, result)));

            // TODO:
            if (_clientVersion == ClientVersion.Lsp2)
            {
                // Small delay to let client respond
                await Task.Delay(100, token);

                _initializeComplete.OnNext(result);
                _initializeComplete.OnCompleted();
            }

            return result;
        }

        public async Task<Unit> Handle(InitializedParams @params, CancellationToken token)
        {
            if (_clientVersion == ClientVersion.Lsp3)
            {
                // Small delay to let client respond
                await Task.Delay(100, token);

                _initializeComplete.OnNext(ServerSettings);
                _initializeComplete.OnCompleted();
            }
            return Unit.Value;
        }

        private async Task DynamicallyRegisterHandlers(Registration[] registrations)
        {
            if (registrations.Length == 0)
                return; // No dynamic registrations supported by client.

            var @params = new RegistrationParams() { Registrations = registrations };

            await _initializeComplete;
            await Client.RegisterCapability(@params);
        }

        public IObservable<bool> Shutdown => _shutdownHandler.Shutdown;
        public IObservable<int> Exit => _exitHandler.Exit;

        public void SendNotification(string method)
        {
            _responseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
        {
            return _responseRouter.SendRequest<T, TResponse>(method, @params);
        }

        public Task<TResponse> SendRequest<TResponse>(string method)
        {
            return _responseRouter.SendRequest<TResponse>(method);
        }

        public Task SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest(method, @params);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            return _responseRouter.GetRequest(id);
        }

        public Task WasShutDown => _shutdownHandler.WasShutDown;
        public Task WaitForExit => _exitHandler.WaitForExit;

        public void Dispose()
        {
            _connection?.Dispose();
            _disposable?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();
    }
}
