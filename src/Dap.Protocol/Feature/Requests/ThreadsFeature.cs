using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using Thread = System.Threading.Thread;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Threads, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class ThreadsArguments : IRequest<ThreadsResponse>
        {
        }

        public class ThreadsResponse
        {
            /// <summary>
            /// All threads.
            /// </summary>
            public Container<Thread>? Threads { get; set; }
        }
    }
}
