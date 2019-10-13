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
    [Serial, Method(DocumentNames.WillSaveWaitUntil)]
    public interface IWillSaveWaitUntilTextDocumentHandler : IJsonRpcRequestHandler<WillSaveWaitUntilTextDocumentParams, Container<TextEdit>>, IRegistration<TextDocumentRegistrationOptions>, ICapability<TextDocumentSyncCapability> { }

    public abstract class WillSaveWaitUntilTextDocumentHandler : IWillSaveWaitUntilTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public WillSaveWaitUntilTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<TextEdit>> Handle(WillSaveWaitUntilTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(TextDocumentSyncCapability capability) => Capability = capability;
        protected TextDocumentSyncCapability Capability { get; private set; }
    }

    public static class WillSaveWaitUntilTextDocumentHandlerExtensions
    {
        public static IDisposable OnWillSaveWaitUntilTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<Container<TextEdit>>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<TextDocumentSyncCapability> setCapability = null)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : WillSaveWaitUntilTextDocumentHandler
        {
            private readonly Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<Container<TextEdit>>> _handler;
            private readonly Action<TextDocumentSyncCapability> _setCapability;

            public DelegatingHandler(
                Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<Container<TextEdit>>> handler,
                Action<TextDocumentSyncCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<TextEdit>> Handle(WillSaveWaitUntilTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(TextDocumentSyncCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
