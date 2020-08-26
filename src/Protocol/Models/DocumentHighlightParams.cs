using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DocumentHighlight, Direction.ClientToServer)]
    public class DocumentHighlightParams : WorkDoneTextDocumentPositionParams, IPartialItemsRequest<DocumentHighlightContainer, DocumentHighlight>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }
    }
}
