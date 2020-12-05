using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.TerminateThreads, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record TerminateThreadsArguments : IRequest<TerminateThreadsResponse>
        {
            /// <summary>
            /// Ids of threads to be terminated.
            /// </summary>
            [Optional]
            public Container<long>? ThreadIds { get; init; }
        }

        public record TerminateThreadsResponse
        {
        }
    }

    namespace Models
    {
    }
}
