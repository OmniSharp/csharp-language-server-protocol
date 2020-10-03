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
    [Method(TextDocumentNames.Declaration, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDeclarationHandler : IJsonRpcRequestHandler<DeclarationParams, LocationOrLocationLinks>, IRegistration<DeclarationRegistrationOptions>,
                                           ICapability<DeclarationCapability>
    {
    }

    public abstract class DeclarationHandler : IDeclarationHandler
    {
        private readonly DeclarationRegistrationOptions _options;
        public DeclarationHandler(DeclarationRegistrationOptions registrationOptions) => _options = registrationOptions;

        public DeclarationRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DeclarationCapability capability) => Capability = capability;
        protected DeclarationCapability Capability { get; private set; } = null!;
    }
}
