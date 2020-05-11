using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class ConfigurationItem
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public DocumentUri ScopeUri { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public string Section { get; set; }
    }
}
