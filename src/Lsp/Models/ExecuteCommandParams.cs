using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ExecuteCommandParams
    {

        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ObjectContainer Arguments { get; set; }
    }
}