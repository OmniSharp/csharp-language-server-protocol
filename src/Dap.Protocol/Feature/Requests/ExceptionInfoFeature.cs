using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.ExceptionInfo, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ExceptionInfoArguments : IRequest<ExceptionInfoResponse>
        {
            /// <summary>
            /// Thread for which exception information should be retrieved.
            /// </summary>
            public long ThreadId { get; init; }
        }

        public record ExceptionInfoResponse
        {
            /// <summary>
            /// ID of the exception that was thrown.
            /// </summary>
            public string ExceptionId { get; init; }

            /// <summary>
            /// Descriptive text for the exception provided by the debug adapter.
            /// </summary>
            [Optional]
            public string? Description { get; init; }

            /// <summary>
            /// Mode that caused the exception notification to be raised.
            /// </summary>
            public ExceptionBreakMode BreakMode { get; init; }

            /// <summary>
            /// Detailed information about the exception.
            /// </summary>
            [Optional]
            public ExceptionDetails? Details { get; init; }
        }
    }
}
