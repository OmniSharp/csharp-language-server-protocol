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
    [Method(TextDocumentNames.FoldingRange, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>,
                                            IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeCapability>
    {
    }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly FoldingRangeRegistrationOptions _options;

        public FoldingRangeHandler(FoldingRangeRegistrationOptions registrationOptions) => _options = registrationOptions;

        public FoldingRangeRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<FoldingRange>> Handle(
            FoldingRangeRequestParam request,
            CancellationToken cancellationToken
        );

        public virtual void SetCapability(FoldingRangeCapability capability) => Capability = capability;
        protected FoldingRangeCapability Capability { get; private set; }
    }
}
