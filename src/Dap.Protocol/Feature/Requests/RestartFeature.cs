using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Restart, Direction.ClientToServer)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record RestartArguments : IRequest<RestartResponse>;

        public record RestartResponse;
    }
}
