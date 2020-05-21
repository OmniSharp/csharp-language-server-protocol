using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Serial, Method(TextDocumentNames.DidChange, Direction.ClientToServer)]
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
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidChangeTextDocumentExtensions
    {
        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidChangeTextDocumentParams, SynchronizationCapability> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidChangeTextDocumentParams, CancellationToken> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidChangeTextDocumentParams> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidChangeTextDocumentParams, SynchronizationCapability, Task> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidChangeTextDocumentParams, CancellationToken, Task> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidChangeTextDocumentParams, Task> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static void DidChangeTextDocument(this ITextDocumentLanguageClient mediator, DidChangeTextDocumentParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
