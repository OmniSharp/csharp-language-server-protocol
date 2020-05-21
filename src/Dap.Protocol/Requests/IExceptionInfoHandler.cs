using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ExceptionInfo, Direction.ClientToServer)]
    public interface IExceptionInfoHandler : IJsonRpcRequestHandler<ExceptionInfoArguments, ExceptionInfoResponse>
    {
    }

    public abstract class ExceptionInfoHandler : IExceptionInfoHandler
    {
        public abstract Task<ExceptionInfoResponse> Handle(ExceptionInfoArguments request,
            CancellationToken cancellationToken);
    }

    public static class ExceptionInfoExtensions
    {
        public static IDisposable OnExceptionInfo(this IDebugAdapterServerRegistry registry,
            Func<ExceptionInfoArguments, CancellationToken, Task<ExceptionInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ExceptionInfo, RequestHandler.For(handler));
        }

        public static IDisposable OnExceptionInfo(this IDebugAdapterServerRegistry registry,
            Func<ExceptionInfoArguments, Task<ExceptionInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ExceptionInfo, RequestHandler.For(handler));
        }

        public static Task<ExceptionInfoResponse> RequestExceptionInfo(this IDebugAdapterClient mediator, ExceptionInfoArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
