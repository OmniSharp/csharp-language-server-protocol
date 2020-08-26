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
    [Method(TextDocumentNames.DocumentHighlight, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, DocumentHighlightContainer?>, IRegistration<DocumentHighlightRegistrationOptions>,
                                                 ICapability<DocumentHighlightCapability>
    {
    }

    public abstract class DocumentHighlightHandler : IDocumentHighlightHandler
    {
        private readonly DocumentHighlightRegistrationOptions _options;
        public DocumentHighlightHandler(DocumentHighlightRegistrationOptions registrationOptions) => _options = registrationOptions;

        public DocumentHighlightRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<DocumentHighlightContainer?> Handle(DocumentHighlightParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentHighlightCapability capability) => Capability = capability;
        protected DocumentHighlightCapability Capability { get; private set; } = null!;
    }
}
