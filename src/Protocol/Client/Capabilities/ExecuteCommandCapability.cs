using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.ExecuteCommand))]
    public class ExecuteCommandCapability : DynamicCapability, ConnectedCapability<IExecuteCommandHandler>
    {
    }
}
