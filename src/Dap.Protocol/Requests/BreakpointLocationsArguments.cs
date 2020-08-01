using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.BreakpointLocations, Direction.ClientToServer)]
    public class BreakpointLocationsArguments : IRequest<BreakpointLocationsResponse> {
        /// <summary>
        /// The source location of the breakpoints; either 'source.path' or 'source.reference' must be specified.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// Start line of range to search possible breakpoint locations in. If only the line is specified, the request returns all possible locations in that line.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Optional start column of range to search possible breakpoint locations in. If no start column is given, the first column in the start line is assumed.
        /// </summary>
        [Optional]
        public int? Column { get; set; }

        /// <summary>
        /// Optional end line of range to search possible breakpoint locations in. If no end line is given, then the end line is assumed to be the start line.
        /// </summary>
        [Optional]
        public int? EndLine { get; set; }

        /// <summary>
        /// Optional end column of range to search possible breakpoint locations in. If no end column is given, then it is assumed to be in the last column of the end line.
        /// </summary>
        [Optional]
        public int? EndColumn { get; set; }
    }
}