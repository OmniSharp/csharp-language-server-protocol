using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Initialized, Direction.ServerToClient)]
        [GenerateHandler(Name = "DebugAdapterInitialized")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record InitializedEvent : IRequest<Unit>;
    }
}
