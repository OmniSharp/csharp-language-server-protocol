using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(GeneralNames.Shutdown)]
    public interface IShutdownHandler : IJsonRpcRequestHandler<EmptyRequest> { }

    public abstract class ShutdownHandler : IShutdownHandler
    {
        public abstract Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken);
    }

    public static class ShutdownHandlerExtensions
    {
        public static IDisposable OnShutdown(this ILanguageServerRegistry registry, Func<Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ShutdownHandler
        {
            private readonly Func<Task<Unit>> _handler;

            public DelegatingHandler(Func<Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken) => _handler.Invoke();
        }
    }
}
