using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Output, Direction.ServerToClient)]
    public class OutputEvent : IRequest
    {
        /// <summary>
        /// The output category. If not specified, 'console' is assumed.
        /// Values: 'console', 'stdout', 'stderr', 'telemetry', etc.
        /// </summary>
        [Optional]
        public string Category { get; set; }

        /// <summary>
        /// The output to report.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// If an attribute 'variablesReference' exists and its value is > 0, the output contains objects which can be retrieved by passing 'variablesReference' to the 'variables' request.
        /// </summary>
        [Optional]
        public long? VariablesReference { get; set; }

        /// <summary>
        /// An optional source location where the output was produced.
        /// </summary>
        [Optional]
        public Source Source { get; set; }

        /// <summary>
        /// An optional source location line where the output was produced.
        /// </summary>
        [Optional]
        public long? Line { get; set; }

        /// <summary>
        /// An optional source location column where the output was produced.
        /// </summary>
        [Optional]
        public long? Column { get; set; }

        /// <summary>
        /// Optional data to report. For the 'telemetry' category the data will be sent to telemetry, for the other categories the data is shown in JSON format.
        /// </summary>
        [Optional]
        public JToken Data { get; set; }
    }
}
