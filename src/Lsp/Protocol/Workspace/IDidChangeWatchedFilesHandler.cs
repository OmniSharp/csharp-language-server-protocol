using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/didChangeWatchedFiles")]
    public interface IDidChangeWatchedFilesHandler : INotificationHandler<DidChangeWatchedFilesParams>, IRegistration<object>, ICapability<DidChangeWatchedFilesCapability> { }
}