using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.RestartFrame, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record RestartFrameArguments : IRequest<RestartFrameResponse>
        {
            /// <summary>
            /// Restart this stackframe.
            /// </summary>
            public long FrameId { get; init; }
        }

        public record RestartFrameResponse
        {
        }
    }
}
