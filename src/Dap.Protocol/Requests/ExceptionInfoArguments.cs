using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ExceptionInfoArguments : IRequest<ExceptionInfoResponse>
    {
        /// <summary>
        /// Thread for which exception information should be retrieved.
        /// </summary>
        public long threadId { get; set; }
    }

}
