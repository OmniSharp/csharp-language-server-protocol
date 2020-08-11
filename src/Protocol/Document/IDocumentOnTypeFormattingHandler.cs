using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Serial]
    [Method(TextDocumentNames.OnTypeFormatting, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentOnTypeFormattingHandler : IJsonRpcRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer>,
                                                        IRegistration<DocumentOnTypeFormattingRegistrationOptions>, ICapability<DocumentOnTypeFormattingCapability>
    {
    }

    public abstract class DocumentOnTypeFormattingHandler : IDocumentOnTypeFormattingHandler
    {
        private readonly DocumentOnTypeFormattingRegistrationOptions _options;
        public DocumentOnTypeFormattingHandler(DocumentOnTypeFormattingRegistrationOptions registrationOptions) => _options = registrationOptions;

        public DocumentOnTypeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentOnTypeFormattingCapability capability) => Capability = capability;
        protected DocumentOnTypeFormattingCapability Capability { get; private set; }
    }
}
