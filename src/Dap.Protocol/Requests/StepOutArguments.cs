using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class StepOutArguments : IRequest<StepOutResponse>
    {
        /// <summary>
        /// Execute 'stepOut' for this thread.
        /// </summary>
        public long threadId { get; set; }
    }

}
