using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Terminate, Direction.ClientToServer)]
    public class TerminateArguments : IRequest<TerminateResponse>
    {
        /// <summary>
        /// A value of true indicates that this 'terminate' request is part of a restart sequence.
        /// </summary>
        [Optional] public bool? Restart { get; set; }
    }

}
