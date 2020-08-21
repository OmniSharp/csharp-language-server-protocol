using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.References))]
    public class ReferenceCapability : DynamicCapability, ConnectedCapability<IReferencesHandler>
    {
    }
}
