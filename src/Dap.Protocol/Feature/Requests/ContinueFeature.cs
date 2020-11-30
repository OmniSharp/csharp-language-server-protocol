using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Continue, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ContinueArguments : IRequest<ContinueResponse>
        {
            /// <summary>
            /// Continue execution for the specified thread(if possible). If the backend cannot continue on a single thread but will continue on all threads, it should set the
            /// 'allThreadsContinued' attribute in the response to true.
            /// </summary>
            public long ThreadId { get; init; }
        }

        public record ContinueResponse
        {
            /// <summary>
            /// If true, the 'continue' request has ignored the specified thread and continued all threads instead.If this attribute is missing a value of 'true' is assumed for backward
            /// compatibility.
            /// </summary>
            [Optional]
            public bool AllThreadsContinued { get; init; }
        }
    }
}
