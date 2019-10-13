using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.DidOpen)]
    public interface IDidOpenTextDocumentHandler : IJsonRpcNotificationHandler<DidOpenTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<TextDocumentSyncClientCapabilities> { }

    public abstract class DidOpenTextDocumentHandler : IDidOpenTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DidOpenTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(TextDocumentSyncClientCapabilities capability) => Capability = capability;
        protected TextDocumentSyncClientCapabilities Capability { get; private set; }
    }

    public static class DidOpenTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidOpenTextDocumentParams, CancellationToken, Task<Unit>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<TextDocumentSyncClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DidOpenTextDocumentHandler
        {
            private readonly Func<DidOpenTextDocumentParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<TextDocumentSyncClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DidOpenTextDocumentParams, CancellationToken, Task<Unit>> handler,
                Action<TextDocumentSyncClientCapabilities> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(TextDocumentSyncClientCapabilities capability) => _setCapability?.Invoke(capability);
        }
    }
}
