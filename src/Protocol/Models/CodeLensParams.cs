using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.CodeLens, Direction.ClientToServer)]
    public class CodeLensParams : ITextDocumentIdentifierParams, IWorkDoneProgressParams, IPartialItemsRequest<CodeLensContainer, CodeLens>
    {
        /// <summary>
        /// The document to request code lens for.
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
