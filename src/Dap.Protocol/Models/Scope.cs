using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// A Scope is a named container for variables.Optionally a scope can map to a source or a range within a source.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Name of the scope such as 'Arguments', 'Locals', or 'Registers'. This string is shown in the UI as is and can be translated.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An optional hint for how to present this scope in the UI. If this attribute is missing, the scope is shown with a generic UI.
        /// Values:
        /// 'arguments': Scope contains method arguments.
        /// 'locals': Scope contains local variables.
        /// 'registers': Scope contains registers. Only a single 'registers' scope should be returned from a 'scopes' request.
        /// etc.
        /// </summary>
        [Optional]
        public string PresentationHint { get; set; }

        /// <summary>
        /// The variables of this scope can be retrieved by passing the value of variablesReference to the VariablesRequest.
        /// </summary>
        public long VariablesReference { get; set; }

        /// <summary>
        /// The long of named variables in this scope.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [Optional]
        public long? NamedVariables { get; set; }

        /// <summary>
        /// The long of indexed variables in this scope.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [Optional]
        public long? IndexedVariables { get; set; }

        /// <summary>
        /// If true, the long of variables in this scope is large or expensive to retrieve.
        /// </summary>
        public bool Expensive { get; set; }

        /// <summary>
        /// Optional source for this scope.
        /// </summary>
        [Optional]
        public Source Source { get; set; }

        /// <summary>
        /// Optional start line of the range covered by this scope.
        /// </summary>
        [Optional]
        public int? Line { get; set; }

        /// <summary>
        /// Optional start column of the range covered by this scope.
        /// </summary>
        [Optional]
        public int? Column { get; set; }

        /// <summary>
        /// Optional end line of the range covered by this scope.
        /// </summary>
        [Optional]
        public int? EndLine { get; set; }

        /// <summary>
        /// Optional end column of the range covered by this scope.
        /// </summary>
        [Optional]
        public int? EndColumn { get; set; }
    }
}
