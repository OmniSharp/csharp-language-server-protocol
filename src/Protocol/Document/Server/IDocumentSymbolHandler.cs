using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.DocumentSymbol)]
    public interface IDocumentSymbolHandler : IJsonRpcRequestHandler<DocumentSymbolParams, Container<SymbolInformationOrDocumentSymbol>>, IRegistration<DocumentSymbolRegistrationOptions>, ICapability<DocumentSymbolClientCapabilities> { }

    public abstract class DocumentSymbolHandler : IDocumentSymbolHandler
    {
        private readonly DocumentSymbolRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public DocumentSymbolHandler(DocumentSymbolRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public DocumentSymbolRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<SymbolInformationOrDocumentSymbol>> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<SymbolInformationOrDocumentSymbol>> Handle(
            DocumentSymbolParams request,
            IObserver<Container<SymbolInformationOrDocumentSymbol>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(DocumentSymbolClientCapabilities capability) => Capability = capability;
        protected DocumentSymbolClientCapabilities Capability { get; private set; }
    }

    public static class DocumentSymbolHandlerExtensions
    {
        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SymbolInformationOrDocumentSymbol>>> handler,
            DocumentSymbolRegistrationOptions registrationOptions = null,
            Action<DocumentSymbolClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentSymbolHandler
        {
            private readonly Func<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SymbolInformationOrDocumentSymbol>>> _handler;
            private readonly Action<DocumentSymbolClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<SymbolInformationOrDocumentSymbol>>> handler,
                ProgressManager progressManager,
                Action<DocumentSymbolClientCapabilities> setCapability,
                DocumentSymbolRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SymbolInformationOrDocumentSymbol>> Handle(
                DocumentSymbolParams request,
                IObserver<Container<SymbolInformationOrDocumentSymbol>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override void SetCapability(DocumentSymbolClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
