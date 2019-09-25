using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class StepOutArguments : IRequest<StepOutResponse>
    {
        /// <summary>
        /// Execute 'stepOut' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
