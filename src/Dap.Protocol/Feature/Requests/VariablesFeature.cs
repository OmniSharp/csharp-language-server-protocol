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
        [Method(RequestNames.Variables, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class VariablesArguments : IRequest<VariablesResponse>
        {
            /// <summary>
            /// The Variable reference.
            /// </summary>
            public long VariablesReference { get; set; }

            /// <summary>
            /// Optional filter to limit the child variables to either named or indexed.If ommited, both types are fetched.
            /// </summary>
            [Optional]
            public VariablesArgumentsFilter? Filter { get; set; }

            /// <summary>
            /// The index of the first variable to return; if omitted children start at 0.
            /// </summary>
            [Optional]
            public long? Start { get; set; }

            /// <summary>
            /// The number of variables to return. If count is missing or 0, all variables are returned.
            /// </summary>
            [Optional]
            public long? Count { get; set; }

            /// <summary>
            /// Specifies details on how to format the Variable values.
            /// </summary>
            [Optional]
            public ValueFormat? Format { get; set; }
        }

        public class VariablesResponse
        {
            /// <summary>
            /// All(or a range) of variables for the given variable reference.
            /// </summary>
            public Container<Variable>? Variables { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum VariablesArgumentsFilter
        {
            Indexed, Named
        }
    }
}
