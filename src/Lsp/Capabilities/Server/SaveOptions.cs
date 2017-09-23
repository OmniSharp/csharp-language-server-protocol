using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Server
{
    /// <summary>
    ///  Save options.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SaveOptions
    {
        /// <summary>
        ///  The client is supposed to include the content on save.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IncludeText { get; set; }
    }
}