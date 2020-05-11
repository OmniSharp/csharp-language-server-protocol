using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class RenameCapability : DynamicCapability, ConnectedCapability<IRenameHandler>
    {
        /// <summary>
        /// Client supports testing for validity of rename operations
        /// before execution.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool PrepareSupport { get; set; }
    }
}
