using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel]
    [Method(TextDocumentNames.DocumentFormatting, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentFormattingHandler : IJsonRpcRequestHandler<DocumentFormattingParams, TextEditContainer?>, IRegistration<DocumentFormattingRegistrationOptions>,
                                                  ICapability<DocumentFormattingCapability>
    {
    }

    public abstract class DocumentFormattingHandler : IDocumentFormattingHandler
    {
        private readonly DocumentFormattingRegistrationOptions _options;
        public DocumentFormattingHandler(DocumentFormattingRegistrationOptions registrationOptions) => _options = registrationOptions;

        public DocumentFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer?> Handle(DocumentFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentFormattingCapability capability) => Capability = capability;
        protected DocumentFormattingCapability Capability { get; private set; } = null!;
    }
}
