using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Abstractions;

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
