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
        [Method(EventNames.Module, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record ModuleEvent : IRequest<Unit>
        {
            /// <summary>
            /// The reason for the event.
            /// </summary>
            public ModuleEventReason Reason { get; init; }

            /// <summary>
            /// The new, changed, or removed module. In case of 'removed' only the module id is used.
            /// </summary>
            public Module Module { get; init; } = null!;
        }

        [StringEnum]
        public readonly partial struct ModuleEventReason
        {
            public static ModuleEventReason Changed { get; } = new ModuleEventReason("changed");
            public static ModuleEventReason New { get; } = new ModuleEventReason("new");
            public static ModuleEventReason Removed { get; } = new ModuleEventReason("removed");
        }
    }
}
