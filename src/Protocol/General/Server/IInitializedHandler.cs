using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(GeneralNames.Initialized)]
    public interface IInitializedHandler : IJsonRpcNotificationHandler<InitializedParams> { }

    public abstract class InitializedHandler : IInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken);
    }

    public static class InitializedHandlerExtensions
    {
        public static IDisposable OnInitialized(this ILanguageServerRegistry registry, Func<InitializedParams, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : InitializedHandler
        {
            private readonly Func<InitializedParams, Task<Unit>> _handler;

            public DelegatingHandler(Func<InitializedParams, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken) => _handler.Invoke(request);

        }
    }
}
