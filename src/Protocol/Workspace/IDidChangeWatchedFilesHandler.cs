using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Serial, Method("workspace/didChangeWatchedFiles")]
    public interface IDidChangeWatchedFilesHandler : INotificationHandler<DidChangeWatchedFilesParams>, IRegistration<object>, ICapability<DidChangeWatchedFilesCapability> { }
}
