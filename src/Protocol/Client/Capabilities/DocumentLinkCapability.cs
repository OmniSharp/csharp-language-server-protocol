using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.DocumentLink))]
    public class DocumentLinkCapability : DynamicCapability, ConnectedCapability<IDocumentLinkHandler>
    {
        /// <summary>
        /// Whether the client support the `tooltip` property on `DocumentLink`.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool TooltipSupport { get; set; }
    }
}
