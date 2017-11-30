using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Serial, Method("exit")]
    public interface IExitHandler : INotificationHandler { }
}
