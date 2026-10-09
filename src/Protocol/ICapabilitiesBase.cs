using System.Collections.Generic;
using System.Text.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ICapabilitiesBase
    {
        IDictionary<string, JsonElement> ExtensionData { get; set; }
    }
}
