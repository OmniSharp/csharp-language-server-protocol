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
    public interface IDocumentSymbolHandler : IJsonRpcRequestHandler<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer>, IRegistration<DocumentSymbolRegistrationOptions>, ICapability<DocumentSymbolCapability> { }

    public abstract class DocumentSymbolHandler : IDocumentSymbolHandler
    {
        private readonly DocumentSymbolRegistrationOptions _options;
        protected ProgressManager ProgressManager { get; }
        public DocumentSymbolHandler(DocumentSymbolRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public DocumentSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentSymbolCapability capability) => Capability = capability;
        protected DocumentSymbolCapability Capability { get; private set; }
    }

    public static class DocumentSymbolHandlerExtensions
    {
        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
            DocumentSymbolRegistrationOptions registrationOptions = null,
            Action<DocumentSymbolCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, registry.ProgressManager, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentSymbolHandler
        {
            private readonly Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> _handler;
            private readonly Action<DocumentSymbolCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
                ProgressManager progressManager,
                Action<DocumentSymbolCapability> setCapability,
                DocumentSymbolRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentSymbolCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
