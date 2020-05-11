using System.Text.Json.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.DocumentHighlight)]
    public class DocumentHighlightParams : WorkDoneTextDocumentPositionParams, IRequest<DocumentHighlightContainer>, IPartialItems<DocumentHighlight>
    {
        /// <inheritdoc />
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken PartialResultToken { get; set; }
    }
}
