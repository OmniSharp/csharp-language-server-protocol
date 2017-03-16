using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CodeLensRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        /// Code lens has a resolve provider as well.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ResolveProvider { get; set; }
    }
}