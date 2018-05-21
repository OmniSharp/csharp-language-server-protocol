using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    using static WorkspaceNames;

    [Serial, Method(DidChangeWatchedFiles)]
    public interface IDidChangeWatchedFilesHandler : IJsonRpcNotificationHandler<DidChangeWatchedFilesParams>, IRegistration<object>, ICapability<DidChangeWatchedFilesCapability> { }
}
