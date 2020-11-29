using System;
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
        [Method(RequestNames.SetBreakpoints, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record SetBreakpointsArguments : IRequest<SetBreakpointsResponse>
        {
            /// <summary>
            /// The source location of the breakpoints; either 'source.path' or 'source.reference' must be specified.
            /// </summary>
            public Source Source { get; init; }

            /// <summary>
            /// The code locations of the breakpoints.
            /// </summary>
            [Optional]
            public Container<SourceBreakpoint>? Breakpoints { get; init; }

            /// <summary>
            /// Deprecated: The code locations of the breakpoints.
            /// </summary>
            [Obsolete("Deprecated")]
            [Optional]
            public Container<long>? Lines { get; init; }

            /// <summary>
            /// A value of true indicates that the underlying source has been modified which results in new breakpoint locations.
            /// </summary>
            [Optional]
            public bool SourceModified { get; init; }
        }

        public record SetBreakpointsResponse
        {
            /// <summary>
            /// Information about the breakpoints.The array elements are in the same order as the elements of the 'breakpoints' (or the deprecated 'lines') array in the arguments.
            /// </summary>
            public Container<Breakpoint> Breakpoints { get; init; }
        }
    }
}
