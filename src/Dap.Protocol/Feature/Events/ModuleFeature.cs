using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Events
    {
        [Parallel]
        [Method(EventNames.Module, Direction.ServerToClient)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class ModuleEvent : IRequest
        {
            /// <summary>
            /// The reason for the event.
            /// </summary>
            public ModuleEventReason Reason { get; set; }

            /// <summary>
            /// The new, changed, or removed module. In case of 'removed' only the module id is used.
            /// </summary>
            public Module Module { get; set; } = null!;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ModuleEventReason
        {
            New, Changed, Removed
        }
    }
}
