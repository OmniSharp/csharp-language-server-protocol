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
    [Parallel, Method(TextDocumentNames.DidClose, Direction.ClientToServer)]
    public interface IDidCloseTextDocumentHandler : IJsonRpcNotificationHandler<DidCloseTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidCloseTextDocumentHandler : IDidCloseTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DidCloseTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidCloseTextDocumentExtensions
    {
        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidCloseTextDocumentParams, SynchronizationCapability> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidCloseTextDocumentParams, CancellationToken> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidCloseTextDocumentParams> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidCloseTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidCloseTextDocumentParams, SynchronizationCapability, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidCloseTextDocumentParams, CancellationToken, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidCloseTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidCloseTextDocumentParams, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidClose,
                new LanguageProtocolDelegatingHandlers.Notification<DidCloseTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static void DidCloseTextDocument(this ITextDocumentLanguageClient mediator, DidCloseTextDocumentParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
