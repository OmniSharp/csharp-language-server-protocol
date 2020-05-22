using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    public interface ITextDocumentSyncHandler : IDidChangeTextDocumentHandler, IDidOpenTextDocumentHandler,
        IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler, ITextDocumentIdentifier
    {
    }

    public abstract class TextDocumentSyncHandler : ITextDocumentSyncHandler
    {
        private readonly TextDocumentSaveRegistrationOptions _options;
        private readonly TextDocumentChangeRegistrationOptions _changeOptions;

        public TextDocumentSyncHandler(TextDocumentSyncKind kind,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
            _changeOptions = new TextDocumentChangeRegistrationOptions() {
                DocumentSelector = registrationOptions.DocumentSelector,
                SyncKind = kind
            };
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions() =>
            _options;

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.
            GetRegistrationOptions() => _options;

        TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions>.
            GetRegistrationOptions() => _changeOptions;

        public abstract TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri);
        public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
        public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
        public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
        public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class TextDocumentSyncExtensions
    {
        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onOpenHandler,
            Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onCloseHandler,
            Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onChangeHandler,
            Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(onOpenHandler, onCloseHandler, onChangeHandler,
                onSaveHandler, getTextDocumentAttributes, registrationOptions, kind));
        }

        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Action<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken> onOpenHandler,
            Action<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken> onCloseHandler,
            Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> onChangeHandler,
            Action<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(onOpenHandler, onCloseHandler, onChangeHandler,
                onSaveHandler, getTextDocumentAttributes, registrationOptions, kind));
        }

        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Action<DidOpenTextDocumentParams, SynchronizationCapability> onOpenHandler,
            Action<DidCloseTextDocumentParams, SynchronizationCapability> onCloseHandler,
            Action<DidChangeTextDocumentParams, SynchronizationCapability> onChangeHandler,
            Action<DidSaveTextDocumentParams, SynchronizationCapability> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(
                (r, c, ct) => onOpenHandler(r, c),
                (r, c, ct) => onCloseHandler(r, c),
                (r, c, ct) => onChangeHandler(r, c),
                (r, c, ct) => onSaveHandler(r, c),
                getTextDocumentAttributes, registrationOptions, kind));
        }

        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Func<DidOpenTextDocumentParams, CancellationToken, Task> onOpenHandler,
            Func<DidCloseTextDocumentParams, CancellationToken, Task> onCloseHandler,
            Func<DidChangeTextDocumentParams, CancellationToken, Task> onChangeHandler,
            Func<DidSaveTextDocumentParams, CancellationToken, Task> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(
                (r, c, ct) => onOpenHandler(r, ct),
                (r, c, ct) => onCloseHandler(r, ct),
                (r, c, ct) => onChangeHandler(r, ct),
                (r, c, ct) => onSaveHandler(r, ct),
                getTextDocumentAttributes, registrationOptions, kind));
        }

        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Action<DidOpenTextDocumentParams, CancellationToken> onOpenHandler,
            Action<DidCloseTextDocumentParams, CancellationToken> onCloseHandler,
            Action<DidChangeTextDocumentParams, CancellationToken> onChangeHandler,
            Action<DidSaveTextDocumentParams, CancellationToken> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(
                (r, c, ct) => onOpenHandler(r, ct),
                (r, c, ct) => onCloseHandler(r, ct),
                (r, c, ct) => onChangeHandler(r, ct),
                (r, c, ct) => onSaveHandler(r, ct),
                getTextDocumentAttributes, registrationOptions, kind));
        }

        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Func<DidOpenTextDocumentParams, Task> onOpenHandler,
            Func<DidCloseTextDocumentParams, Task> onCloseHandler,
            Func<DidChangeTextDocumentParams, Task> onChangeHandler,
            Func<DidSaveTextDocumentParams, Task> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(
                (r, c, ct) => onOpenHandler(r),
                (r, c, ct) => onCloseHandler(r),
                (r, c, ct) => onChangeHandler(r),
                (r, c, ct) => onSaveHandler(r),
                getTextDocumentAttributes, registrationOptions, kind));
        }

        public static IDisposable OnTextDocumentSync(
            this ILanguageServerRegistry registry,
            TextDocumentSyncKind kind,
            Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
            Action<DidOpenTextDocumentParams> onOpenHandler,
            Action<DidCloseTextDocumentParams> onCloseHandler,
            Action<DidChangeTextDocumentParams> onChangeHandler,
            Action<DidSaveTextDocumentParams> onSaveHandler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(
                (r, c, ct) => onOpenHandler(r),
                (r, c, ct) => onCloseHandler(r),
                (r, c, ct) => onChangeHandler(r),
                (r, c, ct) => onSaveHandler(r),
                getTextDocumentAttributes, registrationOptions, kind));
        }

        class DelegatingHandler : TextDocumentSyncHandler
        {
            private readonly Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task>
                _onOpenHandler;

            private readonly Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task>
                _onCloseHandler;

            private readonly Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task>
                _onChangeHandler;

            private readonly Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task>
                _onSaveHandler;

            private readonly Func<DocumentUri, TextDocumentAttributes> _getTextDocumentAttributes;
            private SynchronizationCapability _capability;

            public DelegatingHandler(
                Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onOpenHandler,
                Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onCloseHandler,
                Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onChangeHandler,
                Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> onSaveHandler,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                TextDocumentSaveRegistrationOptions registrationOptions,
                TextDocumentSyncKind kind) : base(kind, registrationOptions)
            {
                _onOpenHandler = onOpenHandler;
                _onSaveHandler = onSaveHandler;
                _onChangeHandler = onChangeHandler;
                _onCloseHandler = onCloseHandler;
                _getTextDocumentAttributes = getTextDocumentAttributes;
            }

            public DelegatingHandler(
                Action<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken> onOpenHandler,
                Action<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken> onCloseHandler,
                Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> onChangeHandler,
                Action<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken> onSaveHandler,
                Func<DocumentUri, TextDocumentAttributes> getTextDocumentAttributes,
                TextDocumentSaveRegistrationOptions registrationOptions,
                TextDocumentSyncKind kind) : this(
                (r, c, ct) => {
                    onOpenHandler(r, c, ct);
                    return Task.CompletedTask;
                },
                (r, c, ct) => {
                    onCloseHandler(r, c, ct);
                    return Task.CompletedTask;
                },
                (r, c, ct) => {
                    onChangeHandler(r, c, ct);
                    return Task.CompletedTask;
                },
                (r, c, ct) => {
                    onSaveHandler(r, c, ct);
                    return Task.CompletedTask;
                },
                getTextDocumentAttributes,
                registrationOptions,
                kind)
            {
            }

            public override async Task<Unit> Handle(DidOpenTextDocumentParams request,
                CancellationToken cancellationToken)
            {
                await _onOpenHandler.Invoke(request, _capability, cancellationToken);
                return Unit.Value;
            }

            public override async Task<Unit> Handle(DidChangeTextDocumentParams request,
                CancellationToken cancellationToken)
            {
                await _onChangeHandler.Invoke(request, _capability, cancellationToken);
                return Unit.Value;
            }

            public override async Task<Unit> Handle(DidSaveTextDocumentParams request,
                CancellationToken cancellationToken)
            {
                await _onSaveHandler.Invoke(request, _capability, cancellationToken);
                return Unit.Value;
            }

            public override async Task<Unit> Handle(DidCloseTextDocumentParams request,
                CancellationToken cancellationToken)
            {
                await _onCloseHandler.Invoke(request, _capability, cancellationToken);
                return Unit.Value;
            }

            public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) =>
                _getTextDocumentAttributes.Invoke(uri);

            public override void SetCapability(SynchronizationCapability capability)
            {
                _capability = capability;
            }
        }
    }
}
