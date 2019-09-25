using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ScopesArguments : IRequest<ScopesResponse>
    {
        /// <summary>
        /// Retrieve the scopes for this stackframe.
        /// </summary>
        public long FrameId { get; set; }
    }

}
