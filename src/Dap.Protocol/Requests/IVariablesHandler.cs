using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Variables, Direction.ClientToServer)]
    public interface IVariablesHandler : IJsonRpcRequestHandler<VariablesArguments, VariablesResponse>
    {
    }

    public abstract class VariablesHandler : IVariablesHandler
    {
        public abstract Task<VariablesResponse> Handle(VariablesArguments request, CancellationToken cancellationToken);
    }

    public static class VariablesExtensions
    {
        public static IDisposable OnVariables(this IDebugAdapterServerRegistry registry,
            Func<VariablesArguments, CancellationToken, Task<VariablesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Variables, RequestHandler.For(handler));
        }

        public static IDisposable OnVariables(this IDebugAdapterServerRegistry registry,
            Func<VariablesArguments, Task<VariablesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Variables, RequestHandler.For(handler));
        }

        public static Task<VariablesResponse> RequestVariables(this IDebugAdapterClient mediator, VariablesArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
