using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Formatting))]
    public class DocumentFormattingCapability : DynamicCapability, ConnectedCapability<IDocumentFormattingHandler>
    {
    }
}
