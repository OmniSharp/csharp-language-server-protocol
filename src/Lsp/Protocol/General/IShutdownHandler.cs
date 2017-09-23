using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Method("shutdown")]
    public interface IShutdownHandler : INotificationHandler { }
}