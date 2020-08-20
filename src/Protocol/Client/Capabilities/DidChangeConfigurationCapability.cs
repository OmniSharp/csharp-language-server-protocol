namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.Configuration))]
    public class DidChangeConfigurationCapability : DynamicCapability
    {
    }
}
