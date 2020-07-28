using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class CancelResponse
    {
        /// <summary>
        /// The ID (attribute 'seq') of the request to cancel. If missing no request is cancelled.
        /// Both a 'requestId' and a 'progressId' can be specified in one request.
        /// </summary>
        [Optional]
        public int? RequestId { get; set; }

        /// <summary>
        /// The ID (attribute 'progressId') of the progress to cancel. If missing no progress is cancelled.
        /// Both a 'requestId' and a 'progressId' can be specified in one request.
        /// </summary>
        [Optional]
        public ProgressToken? ProgressId { get; set; }
    }
}
