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
    [Parallel, Method(DocumentNames.WillSave)]
    public interface IWillSaveTextDocumentHandler : IJsonRpcNotificationHandler<WillSaveTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<TextDocumentSyncClientCapabilities> { }

    public abstract class WillSaveTextDocumentHandler : IWillSaveTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public WillSaveTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(WillSaveTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(TextDocumentSyncClientCapabilities capability) => Capability = capability;
        protected TextDocumentSyncClientCapabilities Capability { get; private set; }
    }

    public static class WillSaveTextDocumentHandlerExtensions
    {
        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveTextDocumentParams, CancellationToken, Task<Unit>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<TextDocumentSyncClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : WillSaveTextDocumentHandler
        {
            private readonly Func<WillSaveTextDocumentParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<TextDocumentSyncClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<WillSaveTextDocumentParams, CancellationToken, Task<Unit>> handler,
                Action<TextDocumentSyncClientCapabilities> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(WillSaveTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(TextDocumentSyncClientCapabilities capability) => _setCapability?.Invoke(capability);
        }
    }
}
