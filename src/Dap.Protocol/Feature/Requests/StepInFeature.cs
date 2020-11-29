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
        [Method(RequestNames.StepIn, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record StepInArguments : IRequest<StepInResponse>
        {
            /// <summary>
            /// Execute 'stepIn' for this thread.
            /// </summary>
            public long ThreadId { get; init; }

            /// <summary>
            /// Optional id of the target to step into.
            /// </summary>
            [Optional]
            public long? TargetId { get; init; }

            /// <summary>
            /// Optional granularity to step. If no granularity is specified, a granularity of 'statement' is assumed.
            /// </summary>
            [Optional]
            public SteppingGranularity? Granularity { get; init; }
        }

        public record StepInResponse
        {
        }
    }

    namespace Models
    {
    }
}
