using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class StepInArguments : IRequest<StepInResponse>
    {
        /// <summary>
        /// Execute 'stepIn' for this thread.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// Optional id of the target to step into.
        /// </summary>
        [Optional] public long? TargetId { get; set; }
    }

}
