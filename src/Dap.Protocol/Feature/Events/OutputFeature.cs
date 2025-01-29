using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Output, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record OutputEvent : IRequest<Unit>
        {
            /// <summary>
            /// The output category. If not specified, 'console' is assumed.
            /// Values: 'console', 'stdout', 'stderr', 'telemetry', etc.
            /// </summary>
            [Optional]
            public OutputEventCategory Category { get; init; } = OutputEventCategory.Console;

            /// <summary>
            /// The output to report.
            /// </summary>
            public string Output { get; init; } = null!;

            /// <summary>
            /// Support for keeping an output log organized by grouping related messages.
            /// Values:
            /// 'start': Start a new group in expanded mode. Subsequent output events are
            /// members of the group and should be shown indented.
            /// The 'output' attribute becomes the name of the group and is not indented.
            /// 'startCollapsed': Start a new group in collapsed mode. Subsequent output
            /// events are members of the group and should be shown indented (as soon as
            /// the group is expanded).
            /// The 'output' attribute becomes the name of the group and is not indented.
            /// 'end': End the current group and decreases the indentation of subsequent
            /// output events.
            /// A non empty 'output' attribute is shown as the unindented end of the
            /// group.
            /// etc.
            /// </summary>
            [Optional]
            public OutputEventGroup Group { get; set; }

            /// <summary>
            /// If an attribute 'variablesReference' exists and its value is > 0, the
            /// output contains objects which can be retrieved by passing
            /// 'variablesReference' to the 'variables' request. The value should be less
            /// than or equal to 2147483647 (2^31-1).
            /// </summary>
            [Optional]
            public long? VariablesReference { get; init; }

            /// <summary>
            /// An optional source location where the output was produced.
            /// </summary>
            [Optional]
            public Source? Source { get; init; }

            /// <summary>
            /// An optional source location line where the output was produced.
            /// </summary>
            [Optional]
            public long? Line { get; init; }

            /// <summary>
            /// An optional source location column where the output was produced.
            /// </summary>
            [Optional]
            public long? Column { get; init; }

            /// <summary>
            /// Optional data to report. For the 'telemetry' category the data will be sent to telemetry, for the other categories the data is shown in JSON format.
            /// </summary>
            [Optional]
            public JToken? Data { get; init; }
        }

        [StringEnum]
        public readonly partial struct OutputEventCategory
        {
            public static OutputEventCategory Console { get; } = new OutputEventCategory("console");
            public static OutputEventCategory StandardOutput { get; } = new OutputEventCategory("stdout");
            public static OutputEventCategory StandardError { get; } = new OutputEventCategory("stderr");
            public static OutputEventCategory Telemetry { get; } = new OutputEventCategory("telemetry");
        }

        [StringEnum]
        public readonly partial struct OutputEventGroup
        {
            public static OutputEventGroup Start { get; } = new OutputEventGroup("start");
            public static OutputEventGroup StartCollapsed { get; } = new OutputEventGroup("startCollapsed");
            public static OutputEventGroup End { get; } = new OutputEventGroup("end");
        }
    }
}
