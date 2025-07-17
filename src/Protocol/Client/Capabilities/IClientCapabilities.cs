using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface IClientCapabilities
    {
        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        IDictionary<string, JToken> Experimental { get; set; }

        IDictionary<string, JToken> ExtensionData { get; set; }
    }
}