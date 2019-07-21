using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Module)]
    public interface IModuleHandler : IJsonRpcNotificationHandler<ModuleEvent> { }

    public abstract class ModuleHandler : IModuleHandler
    {
        public abstract Task<Unit> Handle(ModuleEvent request, CancellationToken cancellationToken);
    }
    public static class ModuleHandlerExtensions
    {
        public static IDisposable OnModule(this IDebugAdapterRegistry registry, Func<ModuleEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ModuleHandler
        {
            private readonly Func<ModuleEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<ModuleEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ModuleEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
