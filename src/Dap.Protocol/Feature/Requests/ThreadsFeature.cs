using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Threads, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ThreadsArguments : IRequest<ThreadsResponse>
        {
        }

        public record ThreadsResponse
        {
            /// <summary>
            /// All threads.
            /// </summary>
            public Container<Thread>? Threads { get; init; }
        }
    }
}
