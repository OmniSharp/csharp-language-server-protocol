using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Models
    {
        /// <summary>
        /// Information about a Breakpoint created in setBreakpoints or setFunctionBreakpoints.
        /// </summary>
        public record Breakpoint
        {
            /// <summary>
            /// An optional identifier for the breakpoint. It is needed if breakpoint events are used to update or remove breakpoints.
            /// </summary>
            [Optional]
            public long? Id { get; init; }

            /// <summary>
            /// If true breakpoint could be set (but not necessarily at the desired location).
            /// </summary>
            public bool Verified { get; init; }

            /// <summary>
            /// An optional message about the state of the breakpoint. This is shown to the user and can be used to explain why a breakpoint could not be verified.
            /// </summary>
            [Optional]
            public string? Message { get; init; }

            /// <summary>
            /// The source where the breakpoint is located.
            /// </summary>
            [Optional]
            public Source? Source { get; init; }

            /// <summary>
            /// The start line of the actual range covered by the breakpoint.
            /// </summary>
            [Optional]
            public int? Line { get; init; }

            /// <summary>
            /// An optional start column of the actual range covered by the breakpoint.
            /// </summary>
            [Optional]
            public int? Column { get; init; }

            /// <summary>
            /// An optional end line of the actual range covered by the breakpoint.
            /// </summary>
            [Optional]
            public int? EndLine { get; init; }

            /// <summary>
            /// An optional end column of the actual range covered by the breakpoint. If no end line is given, then the end column is assumed to be in the start line.
            /// </summary>
            [Optional]
            public int? EndColumn { get; init; }

            /// <summary>
            /// An optional memory reference to where the breakpoint is set.
            /// </summary>
            [Optional]
            public string? InstructionReference { get; init; }

            /// <summary>
            /// An optional offset from the instruction reference.
            /// This can be negative.
            /// </summary>
            [Optional]
            public int? Offset { get; init; }
        }
    }

    namespace Events
    {
        [Parallel]
        [Method(EventNames.Breakpoint, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record BreakpointEvent : IRequest<Unit>
        {
            /// <summary>
            /// The reason for the event.
            /// Values: 'changed', 'new', 'removed', etc.
            /// </summary>
            public BreakpointEventReason Reason { get; init; }

            /// <summary>
            /// The 'id' attribute is used to find the target breakpoint and the other attributes are used as the new values.
            /// </summary>
            public Breakpoint Breakpoint { get; init; } = null!;
        }


        [StringEnum]
        public readonly partial struct BreakpointEventReason
        {
            public static BreakpointEventReason Changed { get; } = new BreakpointEventReason("changed");
            public static BreakpointEventReason New { get; } = new BreakpointEventReason("new");
            public static BreakpointEventReason Removed { get; } = new BreakpointEventReason("removed");
        }
    }
}
