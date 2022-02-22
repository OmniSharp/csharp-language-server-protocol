using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        public abstract record ProgressEvent
        {
            /// <summary>
            /// The ID that was introduced in the initial 'progressStart' event.
            /// </summary>
            public ProgressToken ProgressId { get; init; } = null!;

            /// <summary>
            /// Optional, more detailed progress message. If omitted, the previous message (if any) is used.
            /// </summary>
            [Optional]
            public string? Message { get; init; }
        }

        [Serial]
        [Method(EventNames.ProgressStart, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record ProgressStartEvent : ProgressEvent, IRequest
        {
            /// <summary>
            /// Mandatory (short) title of the progress reporting. Shown in the UI to describe the long running operation.
            /// </summary>
            public string Title { get; init; } = null!;

            /// <summary>
            /// The request ID that this progress report is related to. If specified a debug adapter is expected to emit
            /// progress events for the long running request until the request has been either completed or cancelled.
            /// If the request ID is omitted, the progress report is assumed to be related to some general activity of the debug adapter.
            /// </summary>
            [Optional]
            public int? RequestId { get; init; }

            /// <summary>
            /// If true, the request that reports progress may be canceled with a 'cancel' request.
            /// So this property basically controls whether the client should use UX that supports cancellation.
            /// Clients that don't support cancellation are allowed to ignore the setting.
            /// </summary>
            [Optional]
            public bool Cancellable { get; init; }

            /// <summary>
            /// Optional progress percentage to display (value range: 0 to 100). If omitted no percentage will be shown.
            /// </summary>
            [Optional]
            public int? Percentage { get; init; }
        }

        [Serial]
        [Method(EventNames.ProgressUpdate, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record ProgressUpdateEvent : ProgressEvent, IRequest
        {
            /// <summary>
            /// Optional progress percentage to display (value range: 0 to 100). If omitted no percentage will be shown.
            /// </summary>
            [Optional]
            public double? Percentage { get; init; }
        }

        [Serial]
        [Method(EventNames.ProgressEnd, Direction.ServerToClient)]
        [GenerateHandler]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record ProgressEndEvent : ProgressEvent, IRequest;
    }
}
