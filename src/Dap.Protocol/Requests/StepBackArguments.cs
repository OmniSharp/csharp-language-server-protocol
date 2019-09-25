using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class StepBackArguments : IRequest<StepBackResponse>
    {
        /// <summary>
        /// Execute 'stepBack' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
