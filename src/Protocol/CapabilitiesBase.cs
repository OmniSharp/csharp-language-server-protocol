using System.Collections.Generic;
using System.Text.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public abstract class CapabilitiesBase : ICapabilitiesBase
    {
        public IDictionary<string, JsonElement> ExtensionData { get; set; } = new Dictionary<string, JsonElement>();
    }
}
