using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Serial, Method(TextDocumentNames.WillSaveWaitUntil, Direction.ClientToServer)]
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

    public static class WillSaveWaitUntilTextDocumentExtensions
    {
        public static IDisposable OnWillSaveWaitUntilTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveWaitUntilTextDocumentParams, SynchronizationCapability, CancellationToken, Task<TextEditContainer>>
                handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSaveWaitUntil,
                new LanguageProtocolDelegatingHandlers.Request<WillSaveWaitUntilTextDocumentParams, TextEditContainer, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWillSaveWaitUntilTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveWaitUntilTextDocumentParams, SynchronizationCapability, Task<TextEditContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSaveWaitUntil,
                new LanguageProtocolDelegatingHandlers.Request<WillSaveWaitUntilTextDocumentParams, TextEditContainer, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWillSaveWaitUntilTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveWaitUntilTextDocumentParams, CancellationToken, Task<TextEditContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSaveWaitUntil,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<WillSaveWaitUntilTextDocumentParams, TextEditContainer,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWillSaveWaitUntilTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveWaitUntilTextDocumentParams, Task<TextEditContainer>> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSaveWaitUntil,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<WillSaveWaitUntilTextDocumentParams, TextEditContainer,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<TextEditContainer> RequestWillSaveWaitUntilTextDocument(this ITextDocumentLanguageClient mediator, WillSaveWaitUntilTextDocumentParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
