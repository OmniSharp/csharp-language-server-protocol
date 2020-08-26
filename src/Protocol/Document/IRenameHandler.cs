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
    [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IRenameHandler : IJsonRpcRequestHandler<RenameParams, WorkspaceEdit?>, IRegistration<RenameRegistrationOptions>, ICapability<RenameCapability>
    {
    }

    public abstract class RenameHandler : IRenameHandler
    {
        private readonly RenameRegistrationOptions _options;
        public RenameHandler(RenameRegistrationOptions registrationOptions) => _options = registrationOptions;

        public RenameRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<WorkspaceEdit?> Handle(RenameParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(RenameCapability capability) => Capability = capability;
        protected RenameCapability Capability { get; private set; } = null!;
    }
}
