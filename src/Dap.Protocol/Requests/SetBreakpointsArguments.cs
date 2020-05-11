using System;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SetBreakpointsArguments : IRequest<SetBreakpointsResponse>
    {
        /// <summary>
        /// The source location of the breakpoints; either 'source.path' or 'source.reference' must be specified.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// The code locations of the breakpoints.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<SourceBreakpoint> Breakpoints { get; set; }

        /// <summary>
        /// Deprecated: The code locations of the breakpoints.
        /// </summary>
        [Obsolete("Deprecated")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<long> Lines { get; set; }

        /// <summary>
        /// A value of true indicates that the underlying source has been modified which results in new breakpoint locations.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? SourceModified { get; set; }
    }

}
