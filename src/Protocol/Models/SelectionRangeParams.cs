using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.SelectionRange, Direction.ClientToServer)]
    public class SelectionRangeParams : ITextDocumentIdentifierParams, IPartialItemsRequest<Container<SelectionRange>, SelectionRange>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The positions inside the text document.
        /// </summary>
        public Container<Position> Positions { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
