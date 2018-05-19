using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentLinkCapability : DynamicCapability, ConnectedCapability<IDocumentLinkHandler> { }
}
