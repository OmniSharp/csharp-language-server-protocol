using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.Modules)]
    public interface IModulesHandler : IJsonRpcRequestHandler<ModulesArguments, ModulesResponse> { }

    public abstract class ModulesHandler : IModulesHandler
    {
        public abstract Task<ModulesResponse> Handle(ModulesArguments request, CancellationToken cancellationToken);
    }

    public static class ModulesHandlerExtensions
    {
        public static IDisposable OnModules(this IDebugAdapterRegistry registry, Func<ModulesArguments, CancellationToken, Task<ModulesResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ModulesHandler
        {
            private readonly Func<ModulesArguments, CancellationToken, Task<ModulesResponse>> _handler;

            public DelegatingHandler(Func<ModulesArguments, CancellationToken, Task<ModulesResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ModulesResponse> Handle(ModulesArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
