using System;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class DefaultLanguageServerFacade : LanguageProtocolProxy, ILanguageServerFacade, IOnLanguageServerStarted
    {
        private readonly Lazy<ITextDocumentLanguageServer> _textDocument;
        private readonly Lazy<IClientLanguageServer> _client;
        private readonly Lazy<IGeneralLanguageServer> _general;
        private readonly Lazy<IWindowLanguageServer> _window;
        private readonly Lazy<IWorkspaceLanguageServer> _workspace;
        private readonly Lazy<IHandlersManager> _handlersManager;
        private readonly Lazy<IServerWorkDoneManager> _workDoneManager;
        private readonly Lazy<ISupportedCapabilities> _supportedCapabilities;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        private readonly IInsanceHasStarted _instancesHasStarted;
        private readonly TaskCompletionSource<Unit> _hasStarted;

        public DefaultLanguageServerFacade(
            IResponseRouter requestRouter,
            IResolverContext resolverContext,
            IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings,
            Lazy<ITextDocumentLanguageServer> textDocument,
            Lazy<IClientLanguageServer> client,
            Lazy<IGeneralLanguageServer> general,
            Lazy<IWindowLanguageServer> window,
            Lazy<IWorkspaceLanguageServer> workspace,
            Lazy<IHandlersManager> handlersManager,
            Lazy<IServerWorkDoneManager> workDoneManager,
            Lazy<ISupportedCapabilities> supportedCapabilities,
            TextDocumentIdentifiers textDocumentIdentifiers,
            IInsanceHasStarted instancesHasStarted
        ) : base(requestRouter, resolverContext, progressManager, languageProtocolSettings)
        {
            _textDocument = textDocument;
            _client = client;
            _general = general;
            _window = window;
            _workspace = workspace;
            _handlersManager = handlersManager;
            _workDoneManager = workDoneManager;
            _supportedCapabilities = supportedCapabilities;
            _textDocumentIdentifiers = textDocumentIdentifiers;
            _instancesHasStarted = instancesHasStarted;
            _hasStarted = new TaskCompletionSource<Unit>();
        }

        public ITextDocumentLanguageServer TextDocument => _textDocument.Value;
        public IClientLanguageServer Client => _client.Value;
        public IGeneralLanguageServer General => _general.Value;
        public IWindowLanguageServer Window => _window.Value;
        public IWorkspaceLanguageServer Workspace => _workspace.Value;

        public IDisposable Register(Action<ILanguageServerRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_handlersManager.Value);
            registryAction(new LangaugeServerRegistry(ResolverContext, manager, _textDocumentIdentifiers));

            var result = manager.GetDisposable();
            if (_instancesHasStarted.Started)
            {
                LanguageServerHelpers.InitHandlers(ResolverContext.Resolve<ILanguageServer>(), result);
            }

            return LanguageServerHelpers.RegisterHandlers(_hasStarted.Task, Client, _workDoneManager.Value, _supportedCapabilities.Value, result);
        }

        Task IOnLanguageServerStarted.OnStarted(ILanguageServer client, CancellationToken cancellationToken)
        {
            _hasStarted.TrySetResult(Unit.Value);
            return Task.CompletedTask;
        }
    }
}
