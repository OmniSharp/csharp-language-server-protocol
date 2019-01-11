using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static WorkspaceNames;

    [Parallel, Method(DidChangeWorkspaceFolders)]
    public interface IDidChangeWorkspaceFoldersHandler : IJsonRpcNotificationHandler<DidChangeWorkspaceFoldersParams>, ICapability<DidChangeWorkspaceFolderCapability>, IRegistration<object> { }
}
