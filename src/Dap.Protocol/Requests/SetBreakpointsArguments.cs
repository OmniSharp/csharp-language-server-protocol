using System;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.SetBreakpoints, Direction.ClientToServer)]
    public class SetBreakpointsArguments : IRequest<SetBreakpointsResponse>
    {
        /// <summary>
        /// The source location of the breakpoints; either 'source.path' or 'source.reference' must be specified.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// The code locations of the breakpoints.
        /// </summary>
        [Optional] public Container<SourceBreakpoint> Breakpoints { get; set; }

        /// <summary>
        /// Deprecated: The code locations of the breakpoints.
        /// </summary>
        [Obsolete("Deprecated")]
        [Optional] public Container<long> Lines { get; set; }

        /// <summary>
        /// A value of true indicates that the underlying source has been modified which results in new breakpoint locations.
        /// </summary>
        [Optional] public bool? SourceModified { get; set; }
    }

}
