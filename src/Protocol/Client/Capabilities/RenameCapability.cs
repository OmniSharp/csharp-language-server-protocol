using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Rename))]
    public class RenameCapability : DynamicCapability, ConnectedCapability<IRenameHandler>
    {
        /// <summary>
        /// Client supports testing for validity of rename operations
        /// before execution.
        /// </summary>
        [Optional]
        public bool PrepareSupport { get; set; }

        /// <summary>
        /// Client supports the default behavior result (`{ defaultBehavior: boolean }`).
        ///
        /// @since version 3.16.0
        /// </summary>
        [Optional]
        public bool PrepareSupportDefaultBehavior { get; set; }
    }
}
