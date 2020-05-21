using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Serial, Method(WindowNames.ShowMessageRequest, Direction.ServerToClient)]
    public interface IShowMessageRequestHandler : IJsonRpcRequestHandler<ShowMessageRequestParams, MessageActionItem> { }

    public abstract class ShowMessageRequestHandler : IShowMessageRequestHandler
    {
        public abstract Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken);
    }

    public static class ShowMessageRequestExtensions
    {
        public static IDisposable OnShowMessageRequest(
            this ILanguageClientRegistry registry,
            Func<ShowMessageRequestParams, CancellationToken, Task<MessageActionItem>>
                handler)
        {
            return registry.AddHandler(WindowNames.ShowMessageRequest, RequestHandler.For(handler));
        }

        public static IDisposable OnShowMessageRequest(
            this ILanguageClientRegistry registry,
            Func<ShowMessageRequestParams, Task<MessageActionItem>> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessageRequest, RequestHandler.For(handler));
        }

        public static Task<MessageActionItem> ShowMessageRequest(this IWindowLanguageServer mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
