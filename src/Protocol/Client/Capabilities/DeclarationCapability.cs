using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Declaration))]
    public class DeclarationCapability : LinkSupportCapability, ConnectedCapability<IDeclarationHandler>
    {
    }
}
