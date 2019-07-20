using System;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetBreakpointsArguments : IRequest<SetBreakpointsResponse>
    {
        /// <summary>
        /// The source location of the breakpoints; either 'source.path' or 'source.reference' must be specified.
        /// </summary>
        public Source source { get; set; }

        /// <summary>
        /// The code locations of the breakpoints.
        /// </summary>
        [Optional] public Container<SourceBreakpoint> breakpoints { get; set; }

        /// <summary>
        /// Deprecated: The code locations of the breakpoints.
        /// </summary>
        [Obsolete("Deprecated")]
        [Optional] public Container<long> lines { get; set; }

        /// <summary>
        /// A value of true indicates that the underlying source has been modified which results in new breakpoint locations.
        /// </summary>
        [Optional] public bool? sourceModified { get; set; }
    }

}
