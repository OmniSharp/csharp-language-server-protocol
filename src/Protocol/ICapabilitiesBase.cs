using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ICapabilitiesBase
    {
        [JsonExtensionData] IDictionary<string, JToken> ExtensionData { get; set; }
    }
}
