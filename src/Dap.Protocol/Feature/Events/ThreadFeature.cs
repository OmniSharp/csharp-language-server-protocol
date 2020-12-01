using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
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
        [Method(EventNames.Thread, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record ThreadEvent : IRequest
        {
            /// <summary>
            /// The reason for the event.
            /// Values: 'started', 'exited', etc.
            /// </summary>
            public ThreadEventReason Reason { get; init; }

            /// <summary>
            /// The identifier of the thread.
            /// </summary>
            public long ThreadId { get; init; }
        }

        [StringEnum]
        public readonly partial struct ThreadEventReason
        {
            public static ThreadEventReason Started { get; } = new ThreadEventReason("started");
            public static ThreadEventReason Exited { get; } = new ThreadEventReason("exited");
        }
    }
}
