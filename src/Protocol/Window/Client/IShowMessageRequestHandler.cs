using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(WindowNames.ShowMessageRequest)]
    public interface IShowMessageRequestHandler : IJsonRpcRequestHandler<ShowMessageRequestParams, MessageActionItem> { }

    public abstract class ShowMessageRequestHandler : IShowMessageRequestHandler
    {
        public abstract Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken);
    }

    public static class ShowMessageRequestHandlerExtensions
    {
        public static IDisposable OnShowMessageRequest(
            this ILanguageServerRegistry registry,
            Func<ShowMessageRequestParams, CancellationToken, Task<MessageActionItem>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ShowMessageRequestHandler
        {
            private readonly Func<ShowMessageRequestParams, CancellationToken, Task<MessageActionItem>> _handler;

            public DelegatingHandler(Func<ShowMessageRequestParams, CancellationToken, Task<MessageActionItem>> handler)
            {
                _handler = handler;
            }

            public override Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
