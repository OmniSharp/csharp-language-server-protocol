using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace SampleServer
{
    class FoldingRangeHandler : IFoldingRangeHandler
    {
        private FoldingRangeCapability _capability;

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions() {
                DocumentSelector = DocumentSelector.ForLanguage("csharp")
            };
        }

        public Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new Container<FoldingRange>(new FoldingRange() {
                StartLine = 10,
                EndLine = 20,
                Kind = FoldingRangeKind.Region,
                EndCharacter = 0,
                StartCharacter = 0
            }));
        }

        public void SetCapability(FoldingRangeCapability capability)
        {
            _capability = capability;
        }
    }
}