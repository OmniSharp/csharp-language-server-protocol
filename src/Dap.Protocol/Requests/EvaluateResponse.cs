using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class EvaluateResponse
    {
        /// <summary>
        /// The result of the evaluate request.
        /// </summary>
        public string result { get; set; }

        /// <summary>
        /// The optional type of the evaluate result.
        /// </summary>
        [Optional] public string type { get; set; }

        /// <summary>
        /// Properties of a evaluate result that can be used to determine how to render the result in the UI.
        /// </summary>
        [Optional] public VariablePresentationHint presentationHint { get; set; }

        /// <summary>
        /// If variablesReference is > 0, the evaluate result is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
        /// </summary>
        public long variablesReference { get; set; }

        /// <summary>
        /// The number of named child variables.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [Optional] public long? namedVariables { get; set; }

        /// <summary>
        /// The number of indexed child variables.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [Optional] public long? indexedVariables { get; set; }

        /// <summary>
        /// Memory reference to a location appropriate for this result.For pointer type eval results, this is generally a reference to the memory address contained in the pointer.
        /// </summary>
        [Optional] public string memoryReference { get; set; }
    }

}
