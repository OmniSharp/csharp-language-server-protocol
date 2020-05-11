using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class EvaluateResponse
    {
        /// <summary>
        /// The result of the evaluate request.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// The optional type of the evaluate result.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Type { get; set; }

        /// <summary>
        /// Properties of a evaluate result that can be used to determine how to render the result in the UI.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public VariablePresentationHint PresentationHint { get; set; }

        /// <summary>
        /// If variablesReference is > 0, the evaluate result is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
        /// </summary>
        public long VariablesReference { get; set; }

        /// <summary>
        /// The number of named child variables.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? NamedVariables { get; set; }

        /// <summary>
        /// The number of indexed child variables.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? IndexedVariables { get; set; }

        /// <summary>
        /// Memory reference to a location appropriate for this result.For pointer type eval results, this is generally a reference to the memory address contained in the pointer.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string MemoryReference { get; set; }
    }

}
