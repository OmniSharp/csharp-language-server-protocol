using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.Launch)]
    public interface ILaunchHandler : IJsonRpcRequestHandler<LaunchRequestArguments, LaunchResponse> { }

    public abstract class LaunchHandler : ILaunchHandler
    {
        public abstract Task<LaunchResponse> Handle(LaunchRequestArguments request, CancellationToken cancellationToken);
    }

    public static class LaunchHandlerExtensions
    {
        public static IDisposable OnLaunch(this IDebugAdapterRegistry registry, Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : LaunchHandler
        {
            private readonly Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse>> _handler;

            public DelegatingHandler(Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<LaunchResponse> Handle(LaunchRequestArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
