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
    [Serial, Method(TextDocumentNames.OnTypeFormatting, Direction.ClientToServer)]
    public interface IDocumentOnTypeFormattingHandler : IJsonRpcRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer>, IRegistration<DocumentOnTypeFormattingRegistrationOptions>, ICapability<DocumentOnTypeFormattingCapability> { }

    public abstract class DocumentOnTypeFormattingHandler : IDocumentOnTypeFormattingHandler
    {
        private readonly DocumentOnTypeFormattingRegistrationOptions _options;
        public DocumentOnTypeFormattingHandler(DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentOnTypeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentOnTypeFormattingCapability capability) => Capability = capability;
        protected DocumentOnTypeFormattingCapability Capability { get; private set; }
    }

    public static class DocumentOnTypeFormattingExtensions
    {
        public static IDisposable OnDocumentOnTypeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, DocumentOnTypeFormattingCapability, CancellationToken, Task<TextEditContainer>>
                handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.OnTypeFormatting,
                new LanguageProtocolDelegatingHandlers.Request<DocumentOnTypeFormattingParams, TextEditContainer, DocumentOnTypeFormattingCapability,
                    DocumentOnTypeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentOnTypeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.OnTypeFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentOnTypeFormattingParams, TextEditContainer,
                    DocumentOnTypeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentOnTypeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, Task<TextEditContainer>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.OnTypeFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentOnTypeFormattingParams, TextEditContainer,
                    DocumentOnTypeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<TextEditContainer> RequestDocumentOnTypeFormatting(this ITextDocumentLanguageClient mediator, DocumentOnTypeFormattingParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
