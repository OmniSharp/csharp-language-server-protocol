using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Attach)]
    public interface IAttachHandler : IJsonRpcRequestHandler<AttachRequestArguments, AttachResponse> { }

    public abstract class AttachHandler : IAttachHandler
    {
        public abstract Task<AttachResponse> Handle(AttachRequestArguments request, CancellationToken cancellationToken);
    }

    public static class AttachHandlerExtensions
    {
        public static IDisposable OnAttach(this IDebugAdapterRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : AttachHandler
        {
            private readonly Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> _handler;

            public DelegatingHandler(Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<AttachResponse> Handle(AttachRequestArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
