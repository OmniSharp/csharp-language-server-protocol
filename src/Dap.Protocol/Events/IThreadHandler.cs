using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Thread)]
    public interface IThreadHandler : IJsonRpcNotificationHandler<ThreadEvent> { }

    public abstract class ThreadHandler : IThreadHandler
    {
        public abstract Task<Unit> Handle(ThreadEvent request, CancellationToken cancellationToken);
    }
    public static class ThreadHandlerExtensions
    {
        public static IDisposable OnThread(this IDebugAdapterRegistry registry, Func<ThreadEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ThreadHandler
        {
            private readonly Func<ThreadEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<ThreadEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ThreadEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
