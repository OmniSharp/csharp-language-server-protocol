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
        [Method(RequestNames.StepOut, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class StepOutArguments : IRequest<StepOutResponse>
        {
            /// <summary>
            /// Execute 'stepOut' for this thread.
            /// </summary>
            public long ThreadId { get; set; }

            /// <summary>
            /// Optional granularity to step. If no granularity is specified, a granularity of 'statement' is assumed.
            /// </summary>
            [Optional]
            public SteppingGranularity? Granularity { get; set; }
        }

        public class StepOutResponse
        {
        }
    }

    namespace Models
    {
    }
}
