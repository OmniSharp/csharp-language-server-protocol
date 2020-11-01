using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DocumentColor, Direction.ClientToServer)]
    public class DocumentColorParams : IPartialItemsRequest<Container<ColorInformation>, ColorInformation>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; } = null!;

        /// <inheritdoc />
        [Optional]
        public ProgressToken? PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
