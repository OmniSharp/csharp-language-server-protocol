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
    [Serial, Method(TextDocumentNames.DidOpen, Direction.ClientToServer)]
    public interface IDidOpenTextDocumentHandler : IJsonRpcNotificationHandler<DidOpenTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidOpenTextDocumentHandler : IDidOpenTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DidOpenTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidOpenTextDocumentExtensions
    {
        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidOpenTextDocumentParams, SynchronizationCapability> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidOpenTextDocumentParams> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidOpenTextDocumentParams, CancellationToken> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidOpenTextDocumentParams, SynchronizationCapability, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidOpenTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidOpenTextDocumentParams, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidOpenTextDocumentParams, CancellationToken, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static void DidOpenTextDocument(this ITextDocumentLanguageClient mediator, DidOpenTextDocumentParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
