using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.LoadedSource, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record LoadedSourceEvent : IRequest
        {
            /// <summary>
            /// The reason for the event.
            /// </summary>
            public LoadedSourceReason Reason { get; init; }

            /// <summary>
            /// The new, changed, or removed source.
            /// </summary>
            public Source Source { get; init; }
        }

        [StringEnum]
        public readonly partial struct LoadedSourceReason
        {
            public static readonly LoadedSourceReason Changed = new LoadedSourceReason("changed");
            public static readonly LoadedSourceReason New = new LoadedSourceReason("new");
            public static readonly LoadedSourceReason Removed = new LoadedSourceReason("removed");
        }
    }
}
