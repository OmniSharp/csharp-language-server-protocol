using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DocumentSymbol, Direction.ClientToServer)]
    public class DocumentSymbolParams : ITextDocumentIdentifierParams, IPartialItemsRequest<SymbolInformationOrDocumentSymbolContainer, SymbolInformationOrDocumentSymbol>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
