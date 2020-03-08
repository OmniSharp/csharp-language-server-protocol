using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ITextDocumentSyncHandler : IDidChangeTextDocumentHandler, IDidOpenTextDocumentHandler, IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler, ITextDocumentIdentifier
    {
    }

    public abstract class TextDocumentSyncHandler : ITextDocumentSyncHandler
    {
        private readonly TextDocumentSaveRegistrationOptions _options;
        private readonly TextDocumentChangeRegistrationOptions _changeOptions;

        public TextDocumentSyncHandler(TextDocumentSyncKind kind, TextDocumentSaveRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
            _changeOptions = new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = registrationOptions.DocumentSelector,
                SyncKind = kind
            };
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions() => _options;
        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions() => _options;
        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.GetRegistrationOptions() => _changeOptions;
        public abstract TextDocumentAttributes GetTextDocumentAttributes(Uri uri);
        public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
        public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
        public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
        public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class TextDocumentSyncHandlerExtensions
    {
        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DidOpenTextDocumentParams, CancellationToken, Task<Unit>> onOpenHandler,
            Func<DidCloseTextDocumentParams, CancellationToken, Task<Unit>> onCloseHandler,
            Func<DidChangeTextDocumentParams, CancellationToken, Task<Unit>> onChangeHandler,
            Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> onSaveHandler,
            Func<Uri, TextDocumentAttributes> getTextDocumentAttributes,
            TextDocumentSaveRegistrationOptions registrationOptions = null,
            Action<SynchronizationCapability> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(onOpenHandler, onCloseHandler, onChangeHandler, onSaveHandler, getTextDocumentAttributes, setCapability, registrationOptions, kind));
        }

        class DelegatingHandler : TextDocumentSyncHandler
        {
            private readonly Func<DidOpenTextDocumentParams, CancellationToken, Task<Unit>> _onOpenHandler;
            private readonly Func<DidCloseTextDocumentParams, CancellationToken, Task<Unit>> _onCloseHandler;
            private readonly Func<DidChangeTextDocumentParams, CancellationToken, Task<Unit>> _onChangeHandler;
            private readonly Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> _onSaveHandler;
            private readonly Func<Uri, TextDocumentAttributes> _getTextDocumentAttributes;
            private readonly Action<SynchronizationCapability> _setCapability;

            public DelegatingHandler(
                Func<DidOpenTextDocumentParams, CancellationToken, Task<Unit>> onOpenHandler,
                Func<DidCloseTextDocumentParams, CancellationToken, Task<Unit>> onCloseHandler,
                Func<DidChangeTextDocumentParams, CancellationToken, Task<Unit>> onChangeHandler,
                Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> onSaveHandler,
                Func<Uri, TextDocumentAttributes> getTextDocumentAttributes,
                Action<SynchronizationCapability> setCapability,
                TextDocumentSaveRegistrationOptions registrationOptions,
                TextDocumentSyncKind kind) : base(kind, registrationOptions)
            {
                _onOpenHandler = onOpenHandler;
                _onSaveHandler = onSaveHandler;
                _onChangeHandler = onChangeHandler;
                _onCloseHandler = onCloseHandler;
                _getTextDocumentAttributes = getTextDocumentAttributes;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken) => _onOpenHandler.Invoke(request, cancellationToken);
            public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken) => _onChangeHandler.Invoke(request, cancellationToken);
            public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => _onSaveHandler.Invoke(request, cancellationToken);
            public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken) => _onCloseHandler.Invoke(request, cancellationToken);
            public override TextDocumentAttributes GetTextDocumentAttributes(Uri uri) => _getTextDocumentAttributes.Invoke(uri);
            public override void SetCapability(SynchronizationCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
