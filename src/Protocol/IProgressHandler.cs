using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel, Method(GeneralNames.Progress)]
    public interface IProgressHandler : IJsonRpcNotificationHandler<ProgressParams> { }

    public abstract class ProgressHandler : IProgressHandler
    {
        public abstract Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken);
    }

    public static class ProgressHandlerExtensions
    {
        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Func<ProgressParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }
        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Func<ProgressParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ProgressHandler
        {
            private readonly Func<ProgressParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(
                Func<ProgressParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);

        }
    }
}
