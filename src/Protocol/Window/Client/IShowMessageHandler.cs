using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(WindowNames.ShowMessage)]
    public interface IShowMessageHandler : IJsonRpcNotificationHandler<ShowMessageParams> { }
}
