using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel]
    [Method(TextDocumentNames.DidClose, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDidCloseTextDocumentHandler : IJsonRpcNotificationHandler<DidCloseTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>,
                                                    ICapability<SynchronizationCapability>
    {
    }

    public abstract class DidCloseTextDocumentHandler : IDidCloseTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DidCloseTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions) => _options = registrationOptions;

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }
}
