using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(ClientNames.UnregisterCapability)]
    public interface IUnregisterCapabilityHandler : IJsonRpcRequestHandler<UnregistrationParams> { }

    public abstract class UnregisterCapabilityHandler : IUnregisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(UnregistrationParams request, CancellationToken cancellationToken);
    }

    public static class UnregisterCapabilityHandlerExtensions
    {
        public static IDisposable OnUnregisterCapability(this ILanguageClientRegistry registry, Func<UnregistrationParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : UnregisterCapabilityHandler
        {
            private readonly Func<UnregistrationParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<UnregistrationParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(UnregistrationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
