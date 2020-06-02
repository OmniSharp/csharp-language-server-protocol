using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentLinkCapability : DynamicCapability, ConnectedCapability<IDocumentLinkHandler<CanBeResolvedData>>
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
