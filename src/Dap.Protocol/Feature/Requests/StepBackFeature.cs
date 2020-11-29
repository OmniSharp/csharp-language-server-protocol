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
        [Method(RequestNames.StepBack, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class StepBackArguments : IRequest<StepBackResponse>
        {
            /// <summary>
            /// Execute 'stepBack' for this thread.
            /// </summary>
            public long ThreadId { get; set; }

            /// <summary>
            /// Optional granularity to step. If no granularity is specified, a granularity of 'statement' is assumed.
            /// </summary>
            [Optional]
            public SteppingGranularity? Granularity { get; set; }
        }

        public class StepBackResponse
        {
        }
    }

    namespace Models
    {
    }
}
