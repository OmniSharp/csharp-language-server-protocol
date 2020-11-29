using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Thread, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class ThreadEvent : IRequest
        {
            /// <summary>
            /// The reason for the event.
            /// Values: 'started', 'exited', etc.
            /// </summary>
            public string Reason { get; set; } = null!;

            /// <summary>
            /// The identifier of the thread.
            /// </summary>
            public long ThreadId { get; set; }
        }
    }
}
