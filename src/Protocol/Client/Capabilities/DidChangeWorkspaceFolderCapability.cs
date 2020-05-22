using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DidChangeWorkspaceFolderCapability : DynamicCapability, ConnectedCapability<IDidChangeWorkspaceFoldersHandler> { }
}
