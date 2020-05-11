using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentLinkCapability : DynamicCapability, ConnectedCapability<IDocumentLinkHandler>
    {
        /// <summary>
        /// Whether the client support the `tooltip` property on `DocumentLink`.
        ///
        /// @since 3.15.0
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool TooltipSupport { get; set; }
    }
}
