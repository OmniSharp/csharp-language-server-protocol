using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
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
