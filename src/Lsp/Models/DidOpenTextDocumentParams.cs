using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DidOpenTextDocumentParams
    {
        /// <summary>
        ///  The document that was opened.
        /// </summary>
        public TextDocumentItem TextDocument { get; set; }
    }
}