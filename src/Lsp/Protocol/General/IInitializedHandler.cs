using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Method("initialized")]
    public interface IInitializedHandler : INotificationHandler { }
}