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
    [Serial, Method(TextDocumentNames.DidSave, Direction.ClientToServer)]
    public interface IDidSaveTextDocumentHandler : IJsonRpcNotificationHandler<DidSaveTextDocumentParams>, IRegistration<TextDocumentSaveRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidSaveTextDocumentHandler : IDidSaveTextDocumentHandler
    {
        private readonly TextDocumentSaveRegistrationOptions _options;
        public DidSaveTextDocumentHandler(TextDocumentSaveRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentSaveRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidSaveTextDocumentExtensions
    {
        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidSaveTextDocumentParams, SynchronizationCapability> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams, SynchronizationCapability,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams, SynchronizationCapability,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidSaveTextDocumentParams> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidSaveTextDocumentParams, CancellationToken> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidSaveTextDocumentParams, SynchronizationCapability, Task> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams, SynchronizationCapability,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams, SynchronizationCapability,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidSaveTextDocumentParams, Task> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidSaveTextDocumentParams, CancellationToken, Task> handler,
            TextDocumentSaveRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidSave,
                new LanguageProtocolDelegatingHandlers.Notification<DidSaveTextDocumentParams,
                    TextDocumentSaveRegistrationOptions>(handler, registrationOptions));
        }

        public static void DidSaveTextDocument(this ITextDocumentLanguageClient mediator, DidSaveTextDocumentParams @params)
        {
            mediator.SendNotification(@params);
        }
    }
}
