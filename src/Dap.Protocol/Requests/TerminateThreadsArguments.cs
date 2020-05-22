using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.TerminateThreads, Direction.ClientToServer)]
    public class TerminateThreadsArguments : IRequest<TerminateThreadsResponse>
    {
        /// <summary>
        /// Ids of threads to be terminated.
        /// </summary>
        [Optional] public Container<long> ThreadIds { get; set; }
    }

}
