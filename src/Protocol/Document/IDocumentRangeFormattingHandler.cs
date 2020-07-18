using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Serial, Method(TextDocumentNames.RangeFormatting, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentRangeFormattingHandler : IJsonRpcRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<DocumentRangeFormattingRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }

    public abstract class DocumentRangeFormattingHandler : IDocumentRangeFormattingHandler
    {
        private readonly DocumentRangeFormattingRegistrationOptions _options;
        public DocumentRangeFormattingHandler(DocumentRangeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentRangeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentRangeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentRangeFormattingCapability capability) => Capability = capability;
        protected DocumentRangeFormattingCapability Capability { get; private set; }
    }
}
