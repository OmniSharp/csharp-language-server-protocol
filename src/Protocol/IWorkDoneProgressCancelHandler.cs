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
    [Parallel, Method(WindowNames.WorkDoneProgressCancel)]
    public interface IWorkDoneProgressCancelHandler : IJsonRpcNotificationHandler<WorkDoneProgressCancelParams> { }

    public abstract class WorkDoneProgressCancelHandler : IWorkDoneProgressCancelHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken);
    }

    public static class WorkDoneProgressCancelHandlerExtensions
    {
        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Func<WorkDoneProgressCancelParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }
        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageClientRegistry registry,
            Func<WorkDoneProgressCancelParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : WorkDoneProgressCancelHandler
        {
            private readonly Func<WorkDoneProgressCancelParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(
                Func<WorkDoneProgressCancelParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
