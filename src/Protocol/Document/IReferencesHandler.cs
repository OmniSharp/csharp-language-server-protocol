using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.References, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IReferencesHandler : IJsonRpcRequestHandler<ReferenceParams, LocationContainer>, IRegistration<ReferenceRegistrationOptions>, ICapability<ReferenceCapability> { }

    public abstract class ReferencesHandler : IReferencesHandler
    {
        private readonly ReferenceRegistrationOptions _options;
        public ReferencesHandler(ReferenceRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public ReferenceRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ReferenceCapability capability) => Capability = capability;
        protected ReferenceCapability Capability { get; private set; }
    }
}
