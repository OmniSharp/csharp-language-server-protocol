using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.RunInTerminal, Direction.ServerToClient)]
    public interface IRunInTerminalHandler : IJsonRpcRequestHandler<RunInTerminalArguments, RunInTerminalResponse>
    {
    }

    public abstract class RunInTerminalHandler : IRunInTerminalHandler
    {
        public abstract Task<RunInTerminalResponse> Handle(RunInTerminalArguments request, CancellationToken cancellationToken);
    }

    public static class RunInTerminalExtensions
    {
        public static IDisposable OnRunInTerminal(this IDebugAdapterClientRegistry registry,
            Func<RunInTerminalArguments, CancellationToken, Task<RunInTerminalResponse>> handler)
        {
            return registry.AddHandler(RequestNames.RunInTerminal, RequestHandler.For(handler));
        }

        public static IDisposable OnRunInTerminal(this IDebugAdapterClientRegistry registry,
            Func<RunInTerminalArguments, Task<RunInTerminalResponse>> handler)
        {
            return registry.AddHandler(RequestNames.RunInTerminal, RequestHandler.For(handler));
        }

        public static Task<RunInTerminalResponse> RunInTerminal(this IDebugAdapterServer mediator, RunInTerminalArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }

}
