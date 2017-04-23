using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentFormattingParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The format options.
        /// </summary>
        public FormattingOptions Options { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentRangeFormattingParams
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