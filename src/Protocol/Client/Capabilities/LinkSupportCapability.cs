using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public abstract class LinkSupportCapability : DynamicCapability
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool LinkSupport { get; set; }
    }
}
