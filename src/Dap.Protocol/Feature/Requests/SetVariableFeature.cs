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
        [Method(RequestNames.SetVariable, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record SetVariableArguments : IRequest<SetVariableResponse>
        {
            /// <summary>
            /// The reference of the variable container.
            /// </summary>
            public long VariablesReference { get; init; }

            /// <summary>
            /// The name of the variable in the container.
            /// </summary>
            public string Name { get; init; }

            /// <summary>
            /// The value of the variable.
            /// </summary>
            public string Value { get; init; }

            /// <summary>
            /// Specifies details on how to format the response value.
            /// </summary>
            [Optional]
            public ValueFormat? Format { get; init; }
        }

        public record SetVariableResponse
        {
            /// <summary>
            /// The new value of the variable.
            /// </summary>
            public string Value { get; init; }

            /// <summary>
            /// The type of the new value.Typically shown in the UI when hovering over the value.
            /// </summary>
            [Optional]
            public string? Type { get; init; }

            /// <summary>
            /// If variablesReference is > 0, the new value is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
            /// </summary>
            [Optional]
            public long? VariablesReference { get; init; }

            /// <summary>
            /// The number of named child variables.
            /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
            /// </summary>
            [Optional]
            public long? NamedVariables { get; init; }

            /// <summary>
            /// The number of indexed child variables.
            /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
            /// </summary>
            [Optional]
            public long? IndexedVariables { get; init; }
        }
    }
}
