﻿using System.Threading;
using System.Threading.Tasks;
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
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class SetInstructionBreakpointsArguments : IRequest<SetInstructionBreakpointsResponse>
        {
            /// <summary>
            /// The contents of this array replaces all existing data breakpoints. An empty array clears all data breakpoints.
            /// </summary>
            public Container<DataBreakpoint> Breakpoints { get; set; } = null!;
        }

        public class SetInstructionBreakpointsResponse
        {
            /// <summary>
            /// Information about the data breakpoints.The array elements correspond to the elements of the input argument 'breakpoints' array.
            /// </summary>
            public Container<Breakpoint> Breakpoints { get; set; } = null!;
        }
    }

    namespace Models
    {
    }
}
