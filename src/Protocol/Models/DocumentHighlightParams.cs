using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentHighlightParams : WorkDoneTextDocumentPositionParams, IRequest<Container<DocumentHighlight>>, IPartialItems<DocumentHighlight>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
