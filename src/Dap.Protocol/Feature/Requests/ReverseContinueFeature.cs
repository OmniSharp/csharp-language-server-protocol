using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.ReverseContinue, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class ReverseContinueArguments : IRequest<ReverseContinueResponse>
        {
            /// <summary>
            /// Execute 'reverseContinue' for this thread.
            /// </summary>
            public long ThreadId { get; set; }
        }

        public class ReverseContinueResponse
        {
        }
    }
}
