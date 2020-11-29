using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        [Method(RequestNames.DataBreakpointInfo, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class DataBreakpointInfoArguments : IRequest<DataBreakpointInfoResponse>
        {
            /// <summary>
            /// Reference to the Variable container if the data breakpoint is requested for a child of the container.
            /// </summary>
            [Optional]
            public long? VariablesReference { get; set; }

            /// <summary>
            /// The name of the Variable's child to obtain data breakpoint information for. If variableReference isn’t provided, this can be an expression.
            /// </summary>
            public string Name { get; set; } = null!;
        }

        public class DataBreakpointInfoResponse
        {
            /// <summary>
            /// An identifier for the data on which a data breakpoint can be registered with the setDataBreakpoints request or null if no data breakpoint is available.
            /// </summary>
            public string DataId { get; set; } = null!;

            /// <summary>
            /// UI string that describes on what data the breakpoint is set on or why a data breakpoint is not available.
            /// </summary>
            public string Description { get; set; } = null!;

            /// <summary>
            /// Optional attribute listing the available access types for a potential data breakpoint.A UI frontend could surface this information.
            /// </summary>
            [Optional]
            public Container<DataBreakpointAccessType>? AccessTypes { get; set; }

            /// <summary>
            /// Optional attribute indicating that a potential data breakpoint could be persisted across sessions.
            /// </summary>
            [Optional]
            public bool CanPersist { get; set; }
        }
    }
}
