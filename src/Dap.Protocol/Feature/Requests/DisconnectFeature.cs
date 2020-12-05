using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Disconnect, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record DisconnectArguments : IRequest<DisconnectResponse>
        {
            /// <summary>
            /// A value of true indicates that this 'disconnect' request is part of a restart sequence.
            /// </summary>
            [Optional]
            public bool Restart { get; init; }

            /// <summary>
            /// Indicates whether the debuggee should be terminated when the debugger is disconnected.
            /// If unspecified, the debug adapter is free to do whatever it thinks is best.
            /// A client can only rely on this attribute being properly honored if a debug adapter returns true for the 'supportTerminateDebuggee' capability.
            /// </summary>
            [Optional]
            public bool TerminateDebuggee { get; init; }
        }

        public record DisconnectResponse
        {
        }
    }
}
