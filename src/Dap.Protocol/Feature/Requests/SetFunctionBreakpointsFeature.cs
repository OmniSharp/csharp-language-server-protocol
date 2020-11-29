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
        [Method(RequestNames.SetFunctionBreakpoints, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record SetFunctionBreakpointsArguments : IRequest<SetFunctionBreakpointsResponse>
        {
            /// <summary>
            /// The function names of the breakpoints.
            /// </summary>
            public Container<FunctionBreakpoint> Breakpoints { get; init; }
        }

        public record SetFunctionBreakpointsResponse
        {
            /// <summary>
            /// Information about the breakpoints.The array elements correspond to the elements of the 'breakpoints' array.
            /// </summary>
            public Container<Breakpoint> Breakpoints { get; init; }
        }
    }

    namespace Models
    {
        /// <summary>
        /// FunctionBreakpoint
        /// Properties of a breakpoint passed to the setFunctionBreakpoints request.
        /// </summary>
        public record FunctionBreakpoint
        {
            /// <summary>
            /// The name of the function.
            /// </summary>
            public string Name { get; init; }

            /// <summary>
            /// An optional expression for conditional breakpoints.
            /// </summary>
            [Optional]
            public string? Condition { get; init; }

            /// <summary>
            /// An optional expression that controls how many hits of the breakpoint are ignored. The backend is expected to interpret the expression as needed.
            /// </summary>
            [Optional]
            public string? HitCondition { get; init; }
        }
    }
}
