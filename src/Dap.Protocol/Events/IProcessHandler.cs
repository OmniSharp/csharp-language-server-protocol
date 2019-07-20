using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Process)]
    public interface IProcessHandler : IJsonRpcNotificationHandler<ProcessEvent> { }

    public abstract class ProcessHandler : IProcessHandler
    {
        public abstract Task<Unit> Handle(ProcessEvent request, CancellationToken cancellationToken);
    }
    public static class ProcessHandlerExtensions
    {
        public static IDisposable OnProcess(this IDebugAdapterRegistry registry, Func<ProcessEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ProcessHandler
        {
            private readonly Func<ProcessEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<ProcessEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ProcessEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
