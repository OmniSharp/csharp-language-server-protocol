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
        [Method(RequestNames.SetExpression, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public class SetExpressionArguments : IRequest<SetExpressionResponse>
        {
            /// <summary>
            /// The l-value expression to assign to.
            /// </summary>
            public string Expression { get; set; } = null!;

            /// <summary>
            /// The value expression to assign to the l-value expression.
            /// </summary>
            public string Value { get; set; } = null!;

            /// <summary>
            /// Evaluate the expressions in the scope of this stack frame. If not specified, the expressions are evaluated in the global scope.
            /// </summary>
            [Optional]
            public long? FrameId { get; set; }

            /// <summary>
            /// Specifies how the resulting value should be formatted.
            /// </summary>
            [Optional]
            public ValueFormat? Format { get; set; }
        }

        public class SetExpressionResponse
        {
            /// <summary>
            /// The new value of the expression.
            /// </summary>
            public string Value { get; set; } = null!;

            /// <summary>
            /// The optional type of the value.
            /// </summary>
            [Optional]
            public string? Type { get; set; }

            /// <summary>
            /// Properties of a value that can be used to determine how to render the result in the UI.
            /// </summary>
            [Optional]
            public VariablePresentationHint? PresentationHint { get; set; }

            /// <summary>
            /// If variablesReference is > 0, the value is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
            /// </summary>
            [Optional]
            public long? VariablesReference { get; set; }

            /// <summary>
            /// The number of named child variables.
            /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
            /// </summary>
            [Optional]
            public long? NamedVariables { get; set; }

            /// <summary>
            /// The number of indexed child variables.
            /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
            /// </summary>
            [Optional]
            public long? IndexedVariables { get; set; }
        }
    }
}
