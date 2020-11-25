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
        public class StepInArguments : IRequest<StepInResponse>
        {
            /// <summary>
            /// Execute 'stepIn' for this thread.
            /// </summary>
            public long ThreadId { get; set; }

            /// <summary>
            /// Optional id of the target to step into.
            /// </summary>
            [Optional]
            public long? TargetId { get; set; }

            /// <summary>
            /// Optional granularity to step. If no granularity is specified, a granularity of 'statement' is assumed.
            /// </summary>
            [Optional]
            public SteppingGranularity? Granularity { get; set; }
        }

        public class StepInResponse
        {
        }
    }

    namespace Models
    {
    }
}
