using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.Disassemble)]
    public interface IDisassembleHandler : IJsonRpcRequestHandler<DisassembleArguments, DisassembleResponse> { }

    public abstract class DisassembleHandler : IDisassembleHandler
    {
        public abstract Task<DisassembleResponse> Handle(DisassembleArguments request, CancellationToken cancellationToken);
    }

    public static class DisassembleHandlerExtensions
    {
        public static IDisposable OnDisassemble(this IDebugAdapterRegistry registry, Func<DisassembleArguments, CancellationToken, Task<DisassembleResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : DisassembleHandler
        {
            private readonly Func<DisassembleArguments, CancellationToken, Task<DisassembleResponse>> _handler;

            public DelegatingHandler(Func<DisassembleArguments, CancellationToken, Task<DisassembleResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<DisassembleResponse> Handle(DisassembleArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }

}
