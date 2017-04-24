using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DidSaveTextDocumentParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        ///  The document that was saved.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        ///  Optional the content when saved. Depends on the includeText value
        ///  when the save notifcation was requested.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }
}