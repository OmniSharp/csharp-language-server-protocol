using System.Text.Json.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.DocumentSymbol)]
    public class DocumentSymbolParams : ITextDocumentIdentifierParams, IRequest<SymbolInformationOrDocumentSymbolContainer>, IWorkDoneProgressParams, IPartialItems<SymbolInformationOrDocumentSymbol>
    {
        /// <summary>
        /// The text document.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <inheritdoc />
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken PartialResultToken { get; set; }

        /// <inheritdoc />
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
