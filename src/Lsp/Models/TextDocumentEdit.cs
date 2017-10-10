using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentEdit
    {
        /// <summary>
        /// The text document to change.
        /// </summary>
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The edits to be applied.
        /// </summary>
        public Container<TextEdit> Edits { get; set; }
    }
}
