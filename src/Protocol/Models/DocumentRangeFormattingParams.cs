using OmniSharp.Extensions.Embedded.MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentRangeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>
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
    }
}
