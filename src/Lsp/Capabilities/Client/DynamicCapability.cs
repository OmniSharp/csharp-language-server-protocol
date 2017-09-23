using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DynamicCapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        public bool DynamicRegistration { get; set; }
    }
}
