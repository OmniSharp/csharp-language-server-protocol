using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.ProgressStart, Direction.ServerToClient)]
    public class ProgressStartEvent : ProgressEvent, IRequest
    {
        /// <summary>
        /// Mandatory (short) title of the progress reporting. Shown in the UI to describe the long running operation.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The request ID that this progress report is related to. If specified a debug adapter is expected to emit
        /// progress events for the long running request until the request has been either completed or cancelled.
        /// If the request ID is omitted, the progress report is assumed to be related to some general activity of the debug adapter.
        /// </summary>
        [Optional]
        public int RequestId { get; set; }

        /// <summary>
        /// If true, the request that reports progress may be canceled with a 'cancel' request.
        /// So this property basically controls whether the client should use UX that supports cancellation.
        /// Clients that don't support cancellation are allowed to ignore the setting.
        /// </summary>
        [Optional]
        public bool? Cancellable { get; set; }

        /// <summary>
        /// Optional progress percentage to display (value range: 0 to 100). If omitted no percentage will be shown.
        /// </summary>
        [Optional]
        public int? Percentage { get; set; }
    }
}
