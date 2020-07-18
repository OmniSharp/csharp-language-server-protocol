using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.Hover, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IHoverHandler : IJsonRpcRequestHandler<HoverParams, Hover>, IRegistration<HoverRegistrationOptions>, ICapability<HoverCapability> { }

    public abstract class HoverHandler : IHoverHandler
    {
        private readonly HoverRegistrationOptions _options;
        public HoverHandler(HoverRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public HoverRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(HoverCapability capability) => Capability = capability;
        protected HoverCapability Capability { get; private set; }
    }
}
