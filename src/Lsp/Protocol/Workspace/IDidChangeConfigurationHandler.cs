using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Abstractions;
using OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    [Method("workspace/didChangeConfiguration")]
    public interface IDidChangeConfigurationHandler : INotificationHandler<DidChangeConfigurationParams>, IRegistration<object>, ICapability<DidChangeConfigurationCapability> { }
}