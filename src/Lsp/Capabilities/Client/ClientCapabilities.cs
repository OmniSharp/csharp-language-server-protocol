using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace specific client capabilities.
        /// </summary>
        public WorkspaceClientCapabilites Workspace { get; set; }

        /// <summary>
        /// Text document specific client capabilities.
        /// </summary>
        public TextDocumentClientCapabilities TextDocument { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        public IDictionary<string, object> Experimental { get; set; } = new Dictionary<string, object>();
    }

    public static class ClientCapabilitiesExtensions
    {
        /// <summary>
        /// Best attempt to determine if the hosting client supports a specific protocol version
        ///
        /// ClientCapabilities are new as of 3.0, but the field existsed before so it's possible
        ///     it could be passed as an empty object
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <returns>ClientVersion.</returns>
        public static ClientVersion GetClientVersion(this ClientCapabilities capabilities)
        {
            if (capabilities == null || (capabilities.TextDocument == null && capabilities.Workspace == null))
                return ClientVersion.Lsp2;
            return ClientVersion.Lsp3;
        }
    }

    public enum ClientVersion
    {
        Lsp2,
        Lsp3
    }
}
