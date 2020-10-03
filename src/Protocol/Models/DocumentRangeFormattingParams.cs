using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.RangeFormatting, Direction.ClientToServer)]
    public class DocumentRangeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; } = null!;

        /// <summary>
        /// The range to format
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The format options
        /// </summary>
        public FormattingOptions Options { get; set; } = null!;

        /// <inheritdoc />
        [Optional]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
