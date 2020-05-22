using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Scopes, Direction.ClientToServer)]
    public class ScopesArguments : IRequest<ScopesResponse>
    {
        /// <summary>
        /// Retrieve the scopes for this stackframe.
        /// </summary>
        public long FrameId { get; set; }
    }

}
