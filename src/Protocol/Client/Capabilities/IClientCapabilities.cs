using System.Collections.Generic;
using System.Text.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface IClientCapabilities
    {
        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        IDictionary<string, JsonElement> Experimental { get; set; }

        IDictionary<string, JsonElement> ExtensionData { get; set; }
    }
}