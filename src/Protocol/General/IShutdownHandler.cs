using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Serial, Method("shutdown")]
    public interface IShutdownHandler : INotificationHandler { }
}
