using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CodeLensParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        /// The document to request code lens for.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}