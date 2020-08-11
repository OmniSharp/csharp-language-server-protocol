using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.RestartFrame, Direction.ClientToServer)]
    public class RestartFrameArguments : IRequest<RestartFrameResponse>
    {
        /// <summary>
        /// Restart this stackframe.
        /// </summary>
        public long FrameId { get; set; }
    }
}
