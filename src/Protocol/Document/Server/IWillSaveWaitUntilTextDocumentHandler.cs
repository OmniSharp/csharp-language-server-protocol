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
    public interface IWillSaveWaitUntilTextDocumentHandler : IJsonRpcRequestHandler<WillSaveWaitUntilTextDocumentParams, TextEditContainer>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class WillSaveWaitUntilTextDocumentHandler : IWillSaveWaitUntilTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public WillSaveWaitUntilTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(WillSaveWaitUntilTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class WillSaveWaitUntilTextDocumentHandlerExtensions
    {
        public static IDisposable OnWillSaveWaitUntilTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<TextEditContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions = null,
            Action<SynchronizationCapability> setCapability = null)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : WillSaveWaitUntilTextDocumentHandler
        {
            private readonly Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<TextEditContainer>> _handler;
            private readonly Action<SynchronizationCapability> _setCapability;

            public DelegatingHandler(
                Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<TextEditContainer>> handler,
                Action<SynchronizationCapability> setCapability,
                TextDocumentRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<TextEditContainer> Handle(WillSaveWaitUntilTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SynchronizationCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
