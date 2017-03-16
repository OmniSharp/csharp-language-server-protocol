using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DidCloseTextDocumentParams
    {
        /// <summary>
        ///  The document that was closed.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}