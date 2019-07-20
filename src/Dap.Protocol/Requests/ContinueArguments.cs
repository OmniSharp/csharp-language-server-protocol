using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ContinueArguments : IRequest<ContinueResponse>
    {
        /// <summary>
        /// Continue execution for the specified thread(if possible). If the backend cannot continue on a single thread but will continue on all threads, it should set the 'allThreadsContinued' attribute in the response to true.
        /// </summary>
        public long threadId { get; set; }
    }

}
