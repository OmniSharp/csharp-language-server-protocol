using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ExceptionInfoArguments : IRequest<ExceptionInfoResponse>
    {
        /// <summary>
        /// Thread for which exception information should be retrieved.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
