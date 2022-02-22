using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.SetInstructionBreakpoints, Direction.ClientToServer)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record SetInstructionBreakpointsArguments : IRequest<SetInstructionBreakpointsResponse>
        {
            /// <summary>
            /// The contents of this array replaces all existing data breakpoints. An empty array clears all data breakpoints.
            /// </summary>
            public Container<InstructionBreakpoint> Breakpoints { get; init; } = null!;
        }

        public record SetInstructionBreakpointsResponse
        {
            /// <summary>
            /// Information about the data breakpoints.The array elements correspond to the elements of the input argument 'breakpoints' array.
            /// </summary>
            public Container<Breakpoint> Breakpoints { get; init; } = null!;
        }
    }

    namespace Models
    {
    }
}
