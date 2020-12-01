using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Invalidated, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record InvalidatedEvent : IRequest
        {
            /// <summary>
            /// Optional set of logical areas that got invalidated. This property has a
            /// hint characteristic: a client can only be expected to make a 'best
            /// effort' in honouring the areas but there are no guarantees. If this
            /// property is missing, empty, or if values are not understand the client
            /// should assume a single value 'all'.
            /// </summary>
            [Optional]
            public Container<InvalidatedAreas>? Areas { get; init; }

            /// <summary>
            /// If specified, the client only needs to refetch data related to this
            /// thread.
            /// </summary>
            [Optional]
            public int? ThreadId { get; init; }

            /// <summary>
            /// If specified, the client only needs to refetch data related to this stack
            /// frame (and the 'threadId' is ignored).
            /// </summary>
            [Optional]
            public int? StackFrameId { get; init; }
        }
    }
}
