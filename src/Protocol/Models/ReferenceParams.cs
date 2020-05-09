using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.References)]
    public class ReferenceParams : WorkDoneTextDocumentPositionParams, IRequest<LocationContainer>, IPartialItems<Location>
    {
        public ReferenceContext Context { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
