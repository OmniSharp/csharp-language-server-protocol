using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Terminate, Direction.ClientToServer)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record TerminateArguments : IRequest<TerminateResponse>
        {
            /// <summary>
            /// A value of true indicates that this 'terminate' request is part of a restart sequence.
            /// </summary>
            [Optional]
            public bool Restart { get; init; }
        }

        public record TerminateResponse;
    }

    namespace Models
    {
    }
}
