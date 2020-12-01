using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;

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
        public record StoppedEvent : IRequest
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
            public static readonly StoppedEventReason Step = new StoppedEventReason("step");
            public static readonly StoppedEventReason Breakpoint = new StoppedEventReason("breakpoint");
            public static readonly StoppedEventReason Exception = new StoppedEventReason("exception");
            public static readonly StoppedEventReason Pause = new StoppedEventReason("pause");
            public static readonly StoppedEventReason Entry = new StoppedEventReason("entry");
            public static readonly StoppedEventReason Goto = new StoppedEventReason("goto");
            public static readonly StoppedEventReason FunctionBreakpoint = new StoppedEventReason("function breakpoint");
            public static readonly StoppedEventReason DataBreakpoint = new StoppedEventReason("data breakpoint");
            public static readonly StoppedEventReason InstructionBreakpoint = new StoppedEventReason("instruction breakpoint");
        }
    }
}
