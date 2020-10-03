using System;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    internal class DefaultLanguageClientFacade : LanguageProtocolProxy, ILanguageClientFacade, IOnLanguageClientStarted
    {
        private readonly Lazy<ITextDocumentLanguageClient> _textDocument;
        private readonly Lazy<IClientLanguageClient> _client;
        private readonly Lazy<IGeneralLanguageClient> _general;
        private readonly Lazy<IWindowLanguageClient> _window;
        private readonly Lazy<IWorkspaceLanguageClient> _workspace;
        private readonly Lazy<IHandlersManager> _handlersManager;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;
        private readonly IInsanceHasStarted _insanceHasStarted;
        private ILanguageClient? _languageClient;

        public DefaultLanguageClientFacade(
            IResponseRouter requestRouter,
            IResolverContext resolverContext,
            IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings,
            Lazy<ITextDocumentLanguageClient> textDocument,
            Lazy<IClientLanguageClient> client,
            Lazy<IGeneralLanguageClient> general,
            Lazy<IWindowLanguageClient> window,
            Lazy<IWorkspaceLanguageClient> workspace,
            Lazy<IHandlersManager> handlersManager,
            TextDocumentIdentifiers textDocumentIdentifiers,
            IInsanceHasStarted insanceHasStarted
        ) : base(requestRouter, resolverContext, progressManager, languageProtocolSettings)
        {
            _textDocument = textDocument;
            _client = client;
            _general = general;
            _window = window;
            _workspace = workspace;
            _handlersManager = handlersManager;
            _textDocumentIdentifiers = textDocumentIdentifiers;
            _insanceHasStarted = insanceHasStarted;
        }

        public ITextDocumentLanguageClient TextDocument => _textDocument.Value;
        public IClientLanguageClient Client => _client.Value;
        public IGeneralLanguageClient General => _general.Value;
        public IWindowLanguageClient Window => _window.Value;
        public IWorkspaceLanguageClient Workspace => _workspace.Value;

        public IDisposable Register(Action<ILanguageClientRegistry> registryAction)
        {
            var manager = new CompositeHandlersManager(_handlersManager.Value);
            registryAction(new LangaugeClientRegistry(ResolverContext, manager, _textDocumentIdentifiers));
            var result = manager.GetDisposable();
            if (_insanceHasStarted.Started)
            {
                if (_languageClient == null) throw new NotSupportedException("Language client has not yet started... you shouldn't be here.");
                LanguageClientHelpers.InitHandlers(_languageClient, result);
            }

            return result;
        }

        Task IOnLanguageClientStarted.OnStarted(ILanguageClient client, CancellationToken cancellationToken)
        {
            _languageClient = client;
            return Task.CompletedTask;
        }
    }
}
