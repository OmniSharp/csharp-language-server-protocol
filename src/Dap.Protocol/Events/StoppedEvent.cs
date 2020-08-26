using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Stopped, Direction.ServerToClient)]
    public class StoppedEvent : IRequest
    {
        /// <summary>
        /// The reason for the event.
        /// For backward compatibility this string is shown in the UI if the 'description' attribute is missing (but it must not be translated).
        /// Values: 'step', 'breakpoint', 'exception', 'pause', 'entry', 'goto', 'function breakpoint', 'data breakpoint', etc.
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// The full reason for the event, e.g. 'Paused on exception'. This string is shown in the UI as is and must be translated.
        /// </summary>
        [Optional]
        public string? Description { get; set; }

        /// <summary>
        /// The thread which was stopped.
        /// </summary>
        [Optional]
        public long? ThreadId { get; set; }

        /// <summary>
        /// A value of true hints to the frontend that this event should not change the focus.
        /// </summary>
        [Optional]
        public bool PreserveFocusHint { get; set; }

        /// <summary>
        /// Additional information. E.g. if reason is 'exception', text contains the exception name. This string is shown in the UI.
        /// </summary>
        [Optional]
        public string? Text { get; set; }

        /// <summary>
        /// If 'allThreadsStopped' is true, a debug adapter can announce that all threads have stopped.
        /// - The client should use this information to enable that all threads can be expanded to access their stacktraces.
        /// - If the attribute is missing or false, only the thread with the given threadId can be expanded.
        /// </summary>
        [Optional]
        public bool AllThreadsStopped { get; set; }
    }
}
