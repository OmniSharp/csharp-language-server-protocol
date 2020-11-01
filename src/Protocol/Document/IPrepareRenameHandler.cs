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
    [Method(TextDocumentNames.PrepareRename, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IPrepareRenameHandler : IJsonRpcRequestHandler<PrepareRenameParams, RangeOrPlaceholderRange?>, IRegistration<TextDocumentRegistrationOptions>,
                                             ICapability<RenameCapability>
    {
    }

    public abstract class PrepareRenameHandler : IPrepareRenameHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public PrepareRenameHandler(TextDocumentRegistrationOptions registrationOptions) => _options = registrationOptions;

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<RangeOrPlaceholderRange?> Handle(PrepareRenameParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(RenameCapability capability) => Capability = capability;
        protected RenameCapability Capability { get; private set; } = null!;
    }
}
