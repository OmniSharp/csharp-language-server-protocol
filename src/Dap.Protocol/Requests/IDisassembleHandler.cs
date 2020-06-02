using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Disassemble, Direction.ClientToServer)]
    public interface IDisassembleHandler : IJsonRpcRequestHandler<DisassembleArguments, DisassembleResponse>
    {
    }

    public abstract class DisassembleHandler : IDisassembleHandler
    {
        public abstract Task<DisassembleResponse> Handle(DisassembleArguments request,
            CancellationToken cancellationToken);
    }

    public static class DisassembleExtensions
    {
        public static IDebugAdapterServerRegistry OnDisassemble(this IDebugAdapterServerRegistry registry,
            Func<DisassembleArguments, CancellationToken, Task<DisassembleResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Disassemble, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnDisassemble(this IDebugAdapterServerRegistry registry,
            Func<DisassembleArguments, Task<DisassembleResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Disassemble, RequestHandler.For(handler));
        }

        public static Task<DisassembleResponse> RequestDisassemble(this IDebugAdapterClient mediator, DisassembleArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
