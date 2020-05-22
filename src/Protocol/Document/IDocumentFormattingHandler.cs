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
    [Serial, Method(TextDocumentNames.DocumentFormatting, Direction.ClientToServer)]
    public interface IDocumentFormattingHandler : IJsonRpcRequestHandler<DocumentFormattingParams, TextEditContainer>, IRegistration<DocumentFormattingRegistrationOptions>, ICapability<DocumentFormattingCapability> { }

    public abstract class DocumentFormattingHandler : IDocumentFormattingHandler
    {
        private readonly DocumentFormattingRegistrationOptions _options;
        public DocumentFormattingHandler(DocumentFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentFormattingCapability capability) => Capability = capability;
        protected DocumentFormattingCapability Capability { get; private set; }
    }

    public static class DocumentFormattingExtensions
    {
        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, DocumentFormattingCapability, CancellationToken, Task<TextEditContainer>>
                handler,
            DocumentFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentFormatting,
                new LanguageProtocolDelegatingHandlers.Request<DocumentFormattingParams, TextEditContainer, DocumentFormattingCapability,
                    DocumentFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentFormattingParams, TextEditContainer,
                    DocumentFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, Task<TextEditContainer>> handler,
            DocumentFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentFormattingParams, TextEditContainer,
                    DocumentFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<TextEditContainer> RequestDocumentFormatting(this ITextDocumentLanguageClient mediator, DocumentFormattingParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
