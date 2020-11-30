using System.Threading;
using System.Threading.Tasks;
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
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record OutputEvent : IRequest
        {
            /// <summary>
            /// The output category. If not specified, 'console' is assumed.
            /// Values: 'console', 'stdout', 'stderr', 'telemetry', etc.
            /// </summary>
            [Optional]
            public string? Category { get; init; }

            /// <summary>
            /// The output to report.
            /// </summary>
            public string Output { get; init; }

            /// <summary>
            /// If an attribute 'variablesReference' exists and its value is > 0, the output contains objects which can be retrieved by passing 'variablesReference' to the 'variables' request.
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
    }
}
