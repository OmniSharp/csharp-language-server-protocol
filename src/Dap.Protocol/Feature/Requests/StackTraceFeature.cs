using System.Threading;
using System.Threading.Tasks;
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
        [Method(RequestNames.StackTrace, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class StackTraceArguments : IRequest<StackTraceResponse>
        {
            /// <summary>
            /// Retrieve the stacktrace for this thread.
            /// </summary>
            public long ThreadId { get; set; }

            /// <summary>
            /// The index of the first frame to return; if omitted frames start at 0.
            /// </summary>
            [Optional]
            public long? StartFrame { get; set; }

            /// <summary>
            /// The maximum number of frames to return. If levels is not specified or 0, all frames are returned.
            /// </summary>
            [Optional]
            public long? Levels { get; set; }

            /// <summary>
            /// Specifies details on how to format the stack frames.
            /// </summary>
            [Optional]
            public StackFrameFormat? Format { get; set; }
        }

        public class StackTraceResponse
        {
            /// <summary>
            /// The frames of the stackframe.If the array has length zero, there are no stackframes available.
            /// This means that there is no location information available.
            /// </summary>
            public Container<StackFrame>? StackFrames { get; set; }

            /// <summary>
            /// The total number of frames available.
            /// </summary>
            [Optional]
            public long? TotalFrames { get; set; }
        }
    }

    namespace Models
    {
        /// <summary>
        /// Provides formatting information for a stack frame.
        /// </summary>
        public class StackFrameFormat : ValueFormat
        {
            /// <summary>
            /// Displays parameters for the stack frame.
            /// </summary>
            [Optional]
            public bool Parameters { get; set; }

            /// <summary>
            /// Displays the types of parameters for the stack frame.
            /// </summary>
            [Optional]
            public bool ParameterTypes { get; set; }

            /// <summary>
            /// Displays the names of parameters for the stack frame.
            /// </summary>
            [Optional]
            public bool ParameterNames { get; set; }

            /// <summary>
            /// Displays the values of parameters for the stack frame.
            /// </summary>
            [Optional]
            public bool ParameterValues { get; set; }

            /// <summary>
            /// Displays the line long of the stack frame.
            /// </summary>
            [Optional]
            public bool Line { get; set; }

            /// <summary>
            /// Displays the module of the stack frame.
            /// </summary>
            [Optional]
            public bool Module { get; set; }

            /// <summary>
            /// Includes all stack frames, including those the debug adapter might otherwise hide.
            /// </summary>
            [Optional]
            public bool IncludeAll { get; set; }
        }
    }
}
