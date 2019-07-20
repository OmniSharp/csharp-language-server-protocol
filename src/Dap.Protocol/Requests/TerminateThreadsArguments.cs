using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class TerminateThreadsArguments : IRequest<TerminateThreadsResponse>
    {
        /// <summary>
        /// Ids of threads to be terminated.
        /// </summary>
        [Optional] public Container<long> ThreadIds { get; set; }
    }

}
