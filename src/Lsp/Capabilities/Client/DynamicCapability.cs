using Lsp.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DynamicCapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DynamicRegistration { get; set; }
    }
}