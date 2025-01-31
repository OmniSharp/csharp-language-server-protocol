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
        [Method(EventNames.Stopped, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record StoppedEvent : IRequest<Unit>
        {
            /// <summary>
            /// The reason for the event.
            /// For backward compatibility this string is shown in the UI if the 'description' attribute is missing (but it must not be translated).
            /// Values: 'step', 'breakpoint', 'exception', 'pause', 'entry', 'goto', 'function breakpoint', 'data breakpoint', etc.
            /// </summary>
            public StoppedEventReason Reason { get; init; }

            /// <summary>
            /// The full reason for the event, e.g. 'Paused on exception'. This string is shown in the UI as is and must be translated.
            /// </summary>
            [Optional]
            public string? Description { get; init; }

            /// <summary>
            /// The thread which was stopped.
            /// </summary>
            [Optional]
            public long? ThreadId { get; init; }

            /// <summary>
            /// A value of true hints to the frontend that this event should not change the focus.
            /// </summary>
            [Optional]
            public bool PreserveFocusHint { get; init; }

            /// <summary>
            /// Additional information. E.g. if reason is 'exception', text contains the exception name. This string is shown in the UI.
            /// </summary>
            [Optional]
            public string? Text { get; init; }

            /// <summary>
            /// If 'allThreadsStopped' is true, a debug adapter can announce that all threads have stopped.
            /// - The client should use this information to enable that all threads can be expanded to access their stacktraces.
            /// - If the attribute is missing or false, only the thread with the given threadId can be expanded.
            /// </summary>
            [Optional]
            public bool AllThreadsStopped { get; init; }
        }

        [StringEnum]
        public readonly partial struct StoppedEventReason
        {
            public static StoppedEventReason Step { get; } = new StoppedEventReason("step");
            public static StoppedEventReason Breakpoint { get; } = new StoppedEventReason("breakpoint");
            public static StoppedEventReason Exception { get; } = new StoppedEventReason("exception");
            public static StoppedEventReason Pause { get; } = new StoppedEventReason("pause");
            public static StoppedEventReason Entry { get; } = new StoppedEventReason("entry");
            public static StoppedEventReason Goto { get; } = new StoppedEventReason("goto");
            public static StoppedEventReason FunctionBreakpoint { get; } = new StoppedEventReason("function breakpoint");
            public static StoppedEventReason DataBreakpoint { get; } = new StoppedEventReason("data breakpoint");
            public static StoppedEventReason InstructionBreakpoint { get; } = new StoppedEventReason("instruction breakpoint");
        }
    }
}
