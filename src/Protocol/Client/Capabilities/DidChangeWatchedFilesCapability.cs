namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.DidChangeWatchedFiles))]
    public class DidChangeWatchedFilesCapability : DynamicCapability
    {
    }
}
