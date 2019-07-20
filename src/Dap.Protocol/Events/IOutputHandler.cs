using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Output)]
    public interface IOutputHandler : IJsonRpcNotificationHandler<OutputEvent> { }

    public abstract class OutputHandler : IOutputHandler
    {
        public abstract Task<Unit> Handle(OutputEvent request, CancellationToken cancellationToken);
    }
    public static class OutputHandlerExtensions
    {
        public static IDisposable OnOutput(this IDebugAdapterRegistry registry, Func<OutputEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : OutputHandler
        {
            private readonly Func<OutputEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<OutputEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(OutputEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
