using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.LoadedSource, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record LoadedSourceEvent : IRequest<Unit>
        {
            /// <summary>
            /// The reason for the event.
            /// </summary>
            public LoadedSourceReason Reason { get; init; }

            /// <summary>
            /// The new, changed, or removed source.
            /// </summary>
            public Source Source { get; init; } = null!;
        }

        [StringEnum]
        public readonly partial struct LoadedSourceReason
        {
            public static LoadedSourceReason Changed { get; } = new LoadedSourceReason("changed");
            public static LoadedSourceReason New { get; } = new LoadedSourceReason("new");
            public static LoadedSourceReason Removed { get; } = new LoadedSourceReason("removed");
        }
    }
}
