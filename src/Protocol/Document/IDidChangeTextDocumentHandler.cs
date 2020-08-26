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
    [Serial]
    [Method(TextDocumentNames.DidChange, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
                                                     IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability>
    {
    }

    public abstract class DidChangeTextDocumentHandler : IDidChangeTextDocumentHandler
    {
        private readonly TextDocumentChangeRegistrationOptions _options;
        public DidChangeTextDocumentHandler(TextDocumentChangeRegistrationOptions registrationOptions) => _options = registrationOptions;

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; } = null!;
    }
}
