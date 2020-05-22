using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Modules, Direction.ClientToServer)]
    public interface IModulesHandler : IJsonRpcRequestHandler<ModulesArguments, ModulesResponse>
    {
    }

    public abstract class ModulesHandler : IModulesHandler
    {
        public abstract Task<ModulesResponse> Handle(ModulesArguments request, CancellationToken cancellationToken);
    }

    public static class ModulesExtensions
    {
        public static IDisposable OnModules(this IDebugAdapterServerRegistry registry,
            Func<ModulesArguments, CancellationToken, Task<ModulesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Modules, RequestHandler.For(handler));
        }

        public static IDisposable OnModules(this IDebugAdapterServerRegistry registry,
            Func<ModulesArguments, Task<ModulesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Modules, RequestHandler.For(handler));
        }

        public static Task<ModulesResponse> RequestModules(this IDebugAdapterClient mediator, ModulesArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
