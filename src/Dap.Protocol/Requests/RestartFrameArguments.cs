using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class RestartFrameArguments : IRequest<RestartFrameResponse>
    {
        /// <summary>
        /// Restart this stackframe.
        /// </summary>
        public long FrameId { get; set; }
    }

}
