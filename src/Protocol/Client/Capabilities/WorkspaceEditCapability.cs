using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class WorkspaceEditCapability
    {
        /// <summary>
        /// The client supports versioned document changes in `WorkspaceEdit`s
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DocumentChanges { get; set; }
    }
}
