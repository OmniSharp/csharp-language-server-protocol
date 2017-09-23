using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ApplyWorkspaceEditResponse
    {
        /// <summary>
        /// Indicates whether the edit was applied or not.
        /// </summary>
        public bool Applied { get; set; }
    }
}