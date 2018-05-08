using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Handlers;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerOptions
    {
        public LanguageServerOptions()
        {
        }

        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public ISerializer Serializer { get; set; }
        public IHandlerCollection Handlers { get; set; } = new HandlerCollection();
        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
        public ILspReciever Reciever { get; set; } = new LspReciever();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal bool AddDefaultLoggingProvider { get; set; }
    }

    public static class LanguageServerOptionsExtensions
    {
        public static LanguageServerOptions WithInput(this LanguageServerOptions options, Stream input)
        {
            options.Input = input;
            return options;
        }

        public static LanguageServerOptions WithOutput(this LanguageServerOptions options, Stream output)
        {
            options.Output = output;
            return options;
        }

        public static LanguageServerOptions WithLoggerFactory(this LanguageServerOptions options, ILoggerFactory loggerFactory)
        {
            options.LoggerFactory = loggerFactory;
            return options;
        }

        public static LanguageServerOptions WithRequestProcessIdentifier(this LanguageServerOptions options, IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static LanguageServerOptions WithSerializer(this LanguageServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static LanguageServerOptions WithReciever(this LanguageServerOptions options, ILspReciever reciever)
        {
            options.Reciever = reciever;
            return options;
        }

        public static LanguageServerOptions AddHandler<T>(this LanguageServerOptions options)
            where T : class, IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler, T>();
            return options;
        }

        public static LanguageServerOptions AddHandler<T>(this LanguageServerOptions options, T handler)
            where T : IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler>(handler);
            return options;
        }

        public static LanguageServerOptions AddHandlers(this LanguageServerOptions options, Type type)
        {
            options.HandlerTypes.Add(type);
            return options;
        }

        public static LanguageServerOptions AddHandlers(this LanguageServerOptions options, TypeInfo typeInfo)
        {
            options.HandlerTypes.Add(typeInfo.AsType());
            return options;
        }

        public static LanguageServerOptions AddHandlers(this LanguageServerOptions options, Assembly assembly)
        {
            options.HandlerAssemblies.Add(assembly);
            return options;
        }

        public static LanguageServerOptions AddDefaultLoggingProvider(this LanguageServerOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }
    }

    public class LanguageServer : ILanguageServer, IInitializeHandler, IInitializedHandler, IDisposable, IAwaitableTermination
    {
        private readonly Connection _connection;
        private readonly ILspRequestRouter _requestRouter;
        private readonly ShutdownHandler _shutdownHandler = new ShutdownHandler();
        private readonly List<InitializeDelegate> _initializeDelegates = new List<InitializeDelegate>();
        private readonly ExitHandler _exitHandler;
        private ClientVersion? _clientVersion;
        private readonly ILspReciever _reciever;
        private readonly ISerializer _serializer;
        private readonly IHandlerCollection _collection;
        private readonly IResponseRouter _responseRouter;
        private readonly TaskCompletionSource<InitializeResult> _initializeComplete = new TaskCompletionSource<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public static ILanguageServer From(Action<LanguageServerOptions> optionsAction)
        {
            var options = new LanguageServerOptions();
            optionsAction(options);
            return From(options);
        }

        public static ILanguageServer From(LanguageServerOptions options)
        {
            var server = new LanguageServer(
                options.Input,
                options.Output,
                options.Reciever,
                options.RequestProcessIdentifier,
                options.LoggerFactory,
                options.Serializer,
                options.Handlers,
                options.Services,
                options.HandlerTypes.Select(x => x.Assembly)
                    .Distinct().Concat(options.HandlerAssemblies)
            );

            if (options.AddDefaultLoggingProvider)
                options.LoggerFactory.AddProvider(new LanguageServerLoggerProvider(server));

            return server;
        }

        internal LanguageServer(
            Stream input,
            Stream output,
            ILspReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            ILoggerFactory loggerFactory,
            ISerializer serializer,
            IHandlerCollection handlerCollection,
            IServiceCollection services,
            IEnumerable<Assembly> assemblies)
        {
            var outputHandler = new OutputHandler(output, serializer);

            _reciever = reciever;
            _serializer = serializer;
            _collection = handlerCollection;

            services.AddSingleton<IOutputHandler>(outputHandler);
            services.AddSingleton(handlerCollection);
            services.AddSingleton(serializer);
            services.AddSingleton(requestProcessIdentifier);
            services.AddSingleton(reciever);
            services.AddSingleton(loggerFactory);

            services.AddMediatR(assemblies);
            services.AddScoped<ILspRequestContext, LspRequestContext>();
            services.AddScoped<SingleInstanceFactory>(p => t => {
                var context = p.GetService<ILspRequestContext>();
                return context?.Descriptor != null ? context.Descriptor.Handler : p.GetService(t);
            });
            services.AddTransient<IHandlerMatcher, TextDocumentMatcher>();
            services.AddTransient<IHandlerMatcher, ExecuteCommandMatcher>();
            services.AddTransient<IHandlerMatcher, ResolveCommandMatcher>();
            services.AddSingleton<ILspRequestRouter, LspRequestRouter>();
            services.AddSingleton<IResponseRouter, ResponseRouter>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResolveCommandPipeline<,>));

            var serviceProvider = services.BuildServiceProvider();

            _requestRouter = serviceProvider.GetRequiredService<ILspRequestRouter>();
            _responseRouter = serviceProvider.GetRequiredService<IResponseRouter>();
            _connection = ActivatorUtilities.CreateInstance<Connection>(serviceProvider, input);

            _exitHandler = new ExitHandler(_shutdownHandler);

            _disposable.Add(
                AddHandlers(this, _shutdownHandler, _exitHandler, new CancelRequestHandler(_requestRouter))
            );
        }

        public InitializeParams Client { get; private set; }
        public InitializeResult Server { get; private set; }

        /// <summary>
        ///     The minimum level for the server's default logger.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            var handlerDisposable = _collection.Add(method, handler);

            return new ImmutableDisposable(
                handlerDisposable,
                new Disposable(() => {
                    var foundItems = _collection
                        .Where(x => handler == x.Handler)
                        .Where(x => x.AllowsDynamicRegistration && x.HasRegistration)
                        .Select(x => x.Registration)
                        .ToArray();

                    Task.Run(() => this.UnregisterCapability(new UnregistrationParams() {
                        Unregisterations = foundItems
                    }));
                }));
        }

        public IDisposable AddHandler(IJsonRpcHandler handler)
        {
            return AddHandlers(handler);
        }

        public IDisposable AddHandlers(IEnumerable<IJsonRpcHandler> handlers)
        {
            return AddHandlers(handlers.ToArray());
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            var handlerDisposable = _collection.Add(handlers);

            return new ImmutableDisposable(
                handlerDisposable,
                new Disposable(() => {
                    var foundItems = handlers
                    .SelectMany(handler => _collection
                        .Where(x => handler == x.Handler)
                        .Where(x => x.AllowsDynamicRegistration && x.HasRegistration)
                        .Select(x => x.Registration))
                    .ToArray();

                    Task.Run(() => this.UnregisterCapability(new UnregistrationParams() {
                        Unregisterations = foundItems
                    }));
                }));
        }

        public async Task Initialize()
        {
            _connection.Open();

            await _initializeComplete.Task;

            // Small delay to let client respond
            await Task.Delay(100);

            await DynamicallyRegisterHandlers();
        }

        async Task<InitializeResult> IRequestHandler<InitializeParams, InitializeResult>.Handle(InitializeParams request, CancellationToken token)
        {
            Client = request;

            await Task.WhenAll(_initializeDelegates.Select(c => c(request)));

            _clientVersion = request.Capabilities.GetClientVersion();
            _serializer.SetClientCapabilities(_clientVersion.Value, request.Capabilities);

            if (_clientVersion == ClientVersion.Lsp3)
            {
                // handle client capabilites
                if (request.Capabilities.TextDocument != null)
                {
                    ProcessCapabilties(request.Capabilities.TextDocument);
                }

                if (request.Capabilities.Workspace != null)
                {
                    ProcessCapabilties(request.Capabilities.Workspace);
                }
            }

            var textDocumentCapabilities = Client.Capabilities.TextDocument;
            var workspaceCapabilities = Client.Capabilities.Workspace;

            var ccp = new ClientCapabilityProvider(_collection);

            var serverCapabilities = new ServerCapabilities() {
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
                serverCapabilities.Workspace = new WorkspaceServerCapabilities() {
                    WorkspaceFolders = new WorkspaceFolderOptions() {
                        Supported = true,
                        ChangeNotifications = Guid.NewGuid().ToString()
                    }
                };
            }

            var textSyncHandlers = _collection
                .Select(x => x.Handler)
                .OfType<ITextDocumentSyncHandler>()
                .ToArray();

            if (_clientVersion == ClientVersion.Lsp2)
            {
                if (textSyncHandlers.Any())
                {
                    serverCapabilities.TextDocumentSync = textSyncHandlers
                        .Where(x => x.Options.Change != TextDocumentSyncKind.None)
                        .Min<ITextDocumentSyncHandler, TextDocumentSyncKind>(z => z.Options.Change);
                }
                else
                {
                    serverCapabilities.TextDocumentSync = TextDocumentSyncKind.None;
                }
            }
            else
            {
                if (ccp.HasStaticHandler(textDocumentCapabilities.Synchronization))
                {
                    // TODO: Merge options
                    serverCapabilities.TextDocumentSync =
                        textSyncHandlers.FirstOrDefault()?.Options ?? new TextDocumentSyncOptions() {
                            Change = TextDocumentSyncKind.None,
                            OpenClose = false,
                            Save = new SaveOptions() { IncludeText = false },
                            WillSave = false,
                            WillSaveWaitUntil = false
                        };
                }
                else
                {
                    serverCapabilities.TextDocumentSync = TextDocumentSyncKind.None;
                }
            }

            _reciever.Initialized();

            // TODO: Need a call back here
            // serverCapabilities.Experimental;

            var result = Server = new InitializeResult() { Capabilities = serverCapabilities };

            // TODO:
            if (_clientVersion == ClientVersion.Lsp2)
            {
                _initializeComplete.SetResult(result);
            }

            return result;
        }

        public LanguageServer OnInitialize(InitializeDelegate @delegate)
        {
            _initializeDelegates.Add(@delegate);
            return this;
        }

        public Task Handle(InitializedParams @params, CancellationToken token)
        {
            if (_clientVersion == ClientVersion.Lsp3)
            {
                _initializeComplete.SetResult(Server);
            }
            return Task.CompletedTask;
        }

        private void ProcessCapabilties(object instance)
        {
            var values = instance
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(x => x.CanRead)
                .Select(x => x.GetValue(instance))
                .OfType<ISupports>();

            foreach (var value in values)
            {
                foreach (var handler in _collection.Where(x => x.HasCapability && x.CapabilityType == value.ValueType))
                {
                    handler.SetCapability(value.Value);
                }
            }
        }

        private async Task DynamicallyRegisterHandlers()
        {
            var registrations = _collection
                .Where(x => x.AllowsDynamicRegistration && x.HasRegistration)
                .Select(handler => handler.Registration)
                .ToList();

            if (registrations.Count == 0)
                return; // No dynamic registrations supported by client.

            var @params = new RegistrationParams() { Registrations = registrations };

            await this.RegisterCapability(@params);
        }

        public event ShutdownEventHandler Shutdown
        {
            add => _shutdownHandler.Shutdown += value;
            remove => _shutdownHandler.Shutdown -= value;
        }

        public event ExitEventHandler Exit
        {
            add => _exitHandler.Exit += value;
            remove => _exitHandler.Exit -= value;
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
