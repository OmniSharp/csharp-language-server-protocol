using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using static WorkspaceNames;
    public static partial class WorkspaceNames
    {
        public const string DidChangeConfiguration = "workspace/didChangeConfiguration";
    }

    [Serial, Method(DidChangeConfiguration)]
    public interface IDidChangeConfigurationHandler : INotificationHandler<DidChangeConfigurationParams>, IRegistration<object>, ICapability<DidChangeConfigurationCapability> { }
}
