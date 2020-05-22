using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class HoverCapability : DynamicCapability, ConnectedCapability<IHoverHandler>
    {
        /// <summary>
        /// Client supports the follow content formats for the content property. The order describes the preferred format of the client.
        /// </summary>
        [Optional]
        public Container<MarkupKind> ContentFormat { get; set; }
    }
}
