using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CompletionParams : TextDocumentPositionParams
    {
        /// <summary>
        /// The completion context. This is only available it the client specifies to send
        /// this using `ClientCapabilities.textDocument.completion.contextSupport === true`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CompletionContext Context { get; set; }
    }
}
