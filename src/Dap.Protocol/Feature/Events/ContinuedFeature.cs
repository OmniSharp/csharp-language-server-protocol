using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Continued, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ContinuedEvent : IRequest<Unit>
        {
            /// <summary>
            /// The thread which was continued.
            /// </summary>
            public long ThreadId { get; init; }

            /// <summary>
            /// If 'allThreadsContinued' is true, a debug adapter can announce that all threads have continued.
            /// </summary>
            [Optional]
            public bool AllThreadsContinued { get; init; }
        }
    }
}
