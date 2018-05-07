using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static WorkspaceNames;
    public static partial class WorkspaceNames
    {
        public const string DidChangeWorkspaceFolders = "workspace/didChangeWorkspaceFolders";
    }

    [Parallel, Method(DidChangeWorkspaceFolders)]
    public interface IDidChangeWorkspaceFoldersHandler : INotificationHandler<DidChangeWorkspaceFoldersParams>, ICapability<DidChangeWorkspaceFolderCapability> { }
}
