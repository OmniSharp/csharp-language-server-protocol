using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(DocumentNames.OnTypeFormatting)]
    public class DocumentOnTypeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position at which this request was sent.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// The character that has been typed.
        /// </summary>
        [JsonProperty("ch")]
        public string Character { get; set; }

        /// <summary>
        /// The format options.
        /// </summary>
        public FormattingOptions Options { get; set; }
    }
}
