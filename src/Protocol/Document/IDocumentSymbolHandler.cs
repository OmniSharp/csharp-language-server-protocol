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
    [Method(TextDocumentNames.DocumentSymbol, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentSymbolHandler : IJsonRpcRequestHandler<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer>,
                                              IRegistration<DocumentSymbolRegistrationOptions>, ICapability<DocumentSymbolCapability>
    {
    }

    public abstract class DocumentSymbolHandler : IDocumentSymbolHandler
    {
        private readonly DocumentSymbolRegistrationOptions _options;
        public DocumentSymbolHandler(DocumentSymbolRegistrationOptions registrationOptions) => _options = registrationOptions;

        public DocumentSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentSymbolCapability capability) => Capability = capability;
        protected DocumentSymbolCapability Capability { get; private set; }
    }
}
