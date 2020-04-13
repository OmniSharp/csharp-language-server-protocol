using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.RangeFormatting)]
    public class DocumentRangeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The range to format
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The format options
        /// </summary>
        public FormattingOptions Options { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
