using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class TerminateArguments : IRequest<TerminateResponse>
    {
        /// <summary>
        /// A value of true indicates that this 'terminate' request is part of a restart sequence.
        /// </summary>
        [Optional] public bool? restart { get; set; }
    }

}
