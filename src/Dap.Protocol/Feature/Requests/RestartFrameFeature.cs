using System.Threading;
using System.Threading.Tasks;
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
        public class RestartFrameArguments : IRequest<RestartFrameResponse>
        {
            /// <summary>
            /// Restart this stackframe.
            /// </summary>
            public long FrameId { get; set; }
        }

        public class RestartFrameResponse
        {
        }
    }
}
