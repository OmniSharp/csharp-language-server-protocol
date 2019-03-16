using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel, Method(GeneralNames.CancelRequest)]
    public interface ICancelRequestHandler : IJsonRpcNotificationHandler<CancelParams> { }

    public abstract class CancelRequestHandler : ICancelRequestHandler
    {
        public abstract Task<Unit> Handle(CancelParams request, CancellationToken cancellationToken);
    }

    public static class CancelRequestHandlerExtensions
    {
        public static IDisposable OnCancelRequest(this ILanguageServerRegistry registry, Func<CancelParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }
        public static IDisposable OnCancelRequest(this ILanguageClientRegistry registry, Func<CancelParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : CancelRequestHandler
        {
            private readonly Func<CancelParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<CancelParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(CancelParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
