using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    ///  An event describing a change to a text document. If range and rangeLength are omitted
    ///  the new text is considered to be the full content of the document.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentContentChangeEvent
    {
        /// <summary>
        ///  The range of the document that changed.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Range Range { get; set; }

        /// <summary>
        ///  The length of the range that got replaced.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long RangeLength { get; set; }

        /// <summary>
        ///  The new text of the document.
        /// </summary>
        public string Text { get; set; }
    }
}