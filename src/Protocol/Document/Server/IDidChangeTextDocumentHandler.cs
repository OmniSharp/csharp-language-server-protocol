using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.DidChange)]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
        IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability>
    { }

    public abstract class DidChangeTextDocumentHandler : IDidChangeTextDocumentHandler
    {
        private readonly TextDocumentChangeRegistrationOptions _options;
        public DidChangeTextDocumentHandler(TextDocumentChangeRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(SynchronizationCapability capability);
    }

    public static class DidChangeTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidChangeTextDocumentParams, CancellationToken, Task<Unit>> handler,
            TextDocumentChangeRegistrationOptions registrationOptions = null,
            Action<SynchronizationCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DidChangeTextDocumentHandler
        {
            private readonly Func<DidChangeTextDocumentParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<SynchronizationCapability> _setCapability;

            public DelegatingHandler(
                Func<DidChangeTextDocumentParams, CancellationToken, Task<Unit>> handler,
                Action<SynchronizationCapability> setCapability,
                TextDocumentChangeRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SynchronizationCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
