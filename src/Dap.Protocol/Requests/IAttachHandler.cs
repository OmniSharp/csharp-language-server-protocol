using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Attach, Direction.ClientToServer)]
    public interface IAttachHandler : IJsonRpcRequestHandler<AttachRequestArguments, AttachResponse> { }

    public abstract class AttachHandler : IAttachHandler
    {
        public abstract Task<AttachResponse> Handle(AttachRequestArguments request, CancellationToken cancellationToken);
    }

    public static class AttachExtensions
    {
        public static IDisposable OnAttach(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Attach, RequestHandler.For(handler));
        }

        public static IDisposable OnAttach(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Attach, RequestHandler.For(handler));
        }

        public static Task<AttachResponse> RequestAttach(this IDebugAdapterClient mediator, AttachRequestArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
