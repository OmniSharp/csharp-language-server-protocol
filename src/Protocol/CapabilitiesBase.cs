using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public abstract class CapabilitiesBase : ICapabilitiesBase
    {
        [JsonExtensionData] public IDictionary<string, JToken> ExtensionData { get; set; } = new Dictionary<string, JToken>();
    }
}
