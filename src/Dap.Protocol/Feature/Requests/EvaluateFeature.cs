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
        [Method(RequestNames.Evaluate, Direction.ClientToServer)]
        [
            GenerateHandler,
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record EvaluateArguments : IRequest<EvaluateResponse>
        {
            /// <summary>
            /// The expression to evaluate.
            /// </summary>
            public string Expression { get; init; }

            /// <summary>
            /// Evaluate the expression in the scope of this stack frame. If not specified, the expression is evaluated in the global scope.
            /// </summary>
            [Optional]
            public long? FrameId { get; init; }

            /// <summary>
            /// The context in which the evaluate request is run.
            /// Values:
            /// 'watch': evaluate is run in a watch.
            /// 'repl': evaluate is run from REPL console.
            /// 'hover': evaluate is run from a data hover.
            /// etc.
            /// </summary>
            [Optional]
            public EvaluateArgumentsContext? Context { get; init; }

            /// <summary>
            /// Specifies details on how to format the Evaluate result.
            /// </summary>
            [Optional]
            public ValueFormat? Format { get; init; }
        }

        [StringEnum]
        public readonly partial struct EvaluateArgumentsContext
        {
            public static EvaluateArgumentsContext Watch { get; } = new EvaluateArgumentsContext("watch");
            public static EvaluateArgumentsContext Repl { get; } = new EvaluateArgumentsContext("repl");
            public static EvaluateArgumentsContext Hover { get; } = new EvaluateArgumentsContext("hover");
            public static EvaluateArgumentsContext Clipboard { get; } = new EvaluateArgumentsContext("clipboard");
        }

        public record EvaluateResponse
        {
            /// <summary>
            /// The result of the evaluate request.
            /// </summary>
            public string Result { get; init; }

            /// <summary>
            /// The optional type of the evaluate result.
            /// </summary>
            [Optional]
            public string? Type { get; init; }

            /// <summary>
            /// Properties of a evaluate result that can be used to determine how to render the result in the UI.
            /// </summary>
            [Optional]
            public VariablePresentationHint? PresentationHint { get; init; }

            /// <summary>
            /// If variablesReference is > 0, the evaluate result is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
            /// </summary>
            public long VariablesReference { get; init; }

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

            /// <summary>
            /// Memory reference to a location appropriate for this result.For pointer type eval results, this is generally a reference to the memory address contained in the pointer.
            /// </summary>
            [Optional]
            public string? MemoryReference { get; init; }
        }
    }
}
