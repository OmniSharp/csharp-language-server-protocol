using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class RestartFrameArguments : IRequest<RestartFrameResponse>
    {
        /// <summary>
        /// Restart this stackframe.
        /// </summary>
        public long FrameId { get; set; }
    }

}
