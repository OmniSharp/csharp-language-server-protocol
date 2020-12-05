using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Thread, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ThreadEvent : IRequest
        {
            /// <summary>
            /// The reason for the event.
            /// Values: 'started', 'exited', etc.
            /// </summary>
            public ThreadEventReason Reason { get; init; }

            /// <summary>
            /// The identifier of the thread.
            /// </summary>
            public long ThreadId { get; init; }
        }

        [StringEnum]
        public readonly partial struct ThreadEventReason
        {
            public static ThreadEventReason Started { get; } = new ThreadEventReason("started");
            public static ThreadEventReason Exited { get; } = new ThreadEventReason("exited");
        }
    }
}
