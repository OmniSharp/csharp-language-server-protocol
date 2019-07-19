using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.DidSave)]
    public interface IDidSaveTextDocumentHandler : IJsonRpcNotificationHandler<DidSaveTextDocumentParams>, IRegistration<TextDocumentSaveRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidSaveTextDocumentHandler : IDidSaveTextDocumentHandler
    {
        private readonly TextDocumentSaveRegistrationOptions _options;
        public DidSaveTextDocumentHandler(TextDocumentSaveRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentSaveRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidSaveTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> handler,
            TextDocumentSaveRegistrationOptions registrationOptions = null,
            Action<SynchronizationCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DidSaveTextDocumentHandler
        {
            private readonly Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<SynchronizationCapability> _setCapability;

            public DelegatingHandler(
                Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> handler,
                Action<SynchronizationCapability> setCapability,
                TextDocumentSaveRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SynchronizationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
