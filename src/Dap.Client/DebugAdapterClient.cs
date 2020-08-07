using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Shared;
using OmniSharp.Extensions.JsonRpc;
using IOutputHandler = OmniSharp.Extensions.JsonRpc.IOutputHandler;
using OutputHandler = OmniSharp.Extensions.JsonRpc.OutputHandler;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public class DebugAdapterClient : JsonRpcServerBase, IDebugAdapterClient, IInitializedHandler
    {
        private readonly DebugAdapterSettingsBag _settingsBag;
        private readonly DebugAdapterHandlerCollection _collection;
        private readonly IEnumerable<OnDebugAdapterClientStartedDelegate> _startedDelegates;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly Connection _connection;
        private readonly DapReceiver _receiver;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubject<InitializedEvent> _initializedComplete = new AsyncSubject<InitializedEvent>();

        internal static IContainer CreateContainer(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider) =>
            JsonRpcServerContainer.Create(outerServiceProvider)
                .AddDebugAdapterClientInternals(options, outerServiceProvider);

        public static DebugAdapterClient Create(DebugAdapterClientOptions options) => Create(options, null);
        public static DebugAdapterClient Create(Action<DebugAdapterClientOptions> optionsAction) => Create(optionsAction, null);
        public static DebugAdapterClient Create(Action<DebugAdapterClientOptions> optionsAction, IServiceProvider outerServiceProvider)
        {
            var options = new DebugAdapterClientOptions();
            optionsAction(options);
            return Create(options, outerServiceProvider);
        }

        public static DebugAdapterClient Create(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider) => CreateContainer(options, outerServiceProvider).Resolve<DebugAdapterClient>();

        public static Task<DebugAdapterClient> From(DebugAdapterClientOptions options) => From(options, null, CancellationToken.None);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction) => From(optionsAction, null, CancellationToken.None);
        public static Task<DebugAdapterClient> From(DebugAdapterClientOptions options, CancellationToken cancellationToken) => From(options, null, cancellationToken);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, CancellationToken cancellationToken) => From(optionsAction, null, cancellationToken);
        public static Task<DebugAdapterClient> From(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider) => From(options, outerServiceProvider, CancellationToken.None);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, IServiceProvider outerServiceProvider) => From(optionsAction, outerServiceProvider, CancellationToken.None);
        public static Task<DebugAdapterClient> From(Action<DebugAdapterClientOptions> optionsAction, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var options = new DebugAdapterClientOptions();
            optionsAction(options);
            return From(options, outerServiceProvider, cancellationToken);
        }

        public static async Task<DebugAdapterClient> From(DebugAdapterClientOptions options, IServiceProvider outerServiceProvider, CancellationToken cancellationToken)
        {
            var server = Create(options, outerServiceProvider);
            await server.Initialize(cancellationToken);
            return server;
        }

        internal DebugAdapterClient(
            InitializeRequestArguments clientSettings,
            DebugAdapterSettingsBag settingsBag,
            DebugAdapterHandlerCollection collection,
            IEnumerable<OnDebugAdapterClientStartedDelegate> onClientStartedDelegates,
            DapReceiver receiver,
            IResponseRouter responseRouter,
            IServiceProvider serviceProvider,
            IDebugAdapterClientProgressManager debugAdapterClientProgressManager,
            Connection connection
        ) : base(collection, responseRouter)
        {
            _settingsBag = settingsBag;
            ClientSettings = clientSettings;
            _collection = collection;
            _startedDelegates = onClientStartedDelegates;
            _receiver = receiver;
            _serviceProvider = serviceProvider;
            ProgressManager = debugAdapterClientProgressManager;
            _connection = connection;
            _disposable.Add(collection.Add(this));
        }

        public async Task Initialize(CancellationToken token)
        {
            RegisterCapabilities(ClientSettings);

            _connection.Open();
            var serverParams = await this.RequestInitialize(ClientSettings, token);

            ServerSettings = serverParams;
            _receiver.Initialized();

            await _initializedComplete.ToTask(token);

            await _startedDelegates.Select(@delegate => Observable.FromAsync(() => @delegate(this, serverParams, token)))
                .ToObservable()
                .Merge()
                .LastOrDefaultAsync()
                .ToTask(token);
        }

        Task<Unit> IRequestHandler<InitializedEvent, Unit>.Handle(InitializedEvent request, CancellationToken cancellationToken)
        {
            _initializedComplete.OnNext(request);
            _initializedComplete.OnCompleted();
            return Unit.Task;
        }

        private void RegisterCapabilities(InitializeRequestArguments capabilities)
        {
            capabilities.SupportsRunInTerminalRequest ??= _collection.ContainsHandler(typeof(IRunInTerminalHandler));
            capabilities.SupportsProgressReporting ??= _collection.ContainsHandler(typeof(IProgressStartHandler)) &&
                                                       _collection.ContainsHandler(typeof(IProgressUpdateHandler)) &&
                                                       _collection.ContainsHandler(typeof(IProgressEndHandler));
        }

        public InitializeRequestArguments ClientSettings
        {
            get => _settingsBag.ClientSettings;
            private set => _settingsBag.ClientSettings = value;
        }

        public InitializeResponse ServerSettings
        {
            get => _settingsBag.ServerSettings;
            private set => _settingsBag.ServerSettings = value;
        }
        public IDebugAdapterClientProgressManager ProgressManager { get; }

        public void Dispose()
        {
            _disposable?.Dispose();
            _connection?.Dispose();
        }

        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
