using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentOnTypeFormattingParams : ITextDocumentIdentifierParams
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
