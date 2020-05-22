using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetVariable, Direction.ClientToServer)]
    public interface ISetVariableHandler : IJsonRpcRequestHandler<SetVariableArguments, SetVariableResponse>
    {
    }

    public abstract class SetVariableHandler : ISetVariableHandler
    {
        public abstract Task<SetVariableResponse> Handle(SetVariableArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetVariableExtensions
    {
        public static IDisposable OnSetVariable(this IDebugAdapterServerRegistry registry,
            Func<SetVariableArguments, CancellationToken, Task<SetVariableResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetVariable, RequestHandler.For(handler));
        }

        public static IDisposable OnSetVariable(this IDebugAdapterServerRegistry registry,
            Func<SetVariableArguments, Task<SetVariableResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetVariable, RequestHandler.For(handler));
        }

        public static Task<SetVariableResponse> RequestSetVariable(this IDebugAdapterClient mediator, SetVariableArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
