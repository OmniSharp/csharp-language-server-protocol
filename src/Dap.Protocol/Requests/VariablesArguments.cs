using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class VariablesArguments : IRequest<VariablesResponse>
    {
        /// <summary>
        /// The Variable reference.
        /// </summary>
        public long variablesReference { get; set; }

        /// <summary>
        /// Optional filter to limit the child variables to either named or indexed.If ommited, both types are fetched.
        /// </summary>
        [Optional] public VariablesArgumentsFilter filter { get; set; }

        /// <summary>
        /// The index of the first variable to return; if omitted children start at 0.
        /// </summary>
        [Optional] public long? start { get; set; }

        /// <summary>
        /// The number of variables to return. If count is missing or 0, all variables are returned.
        /// </summary>
        [Optional] public long? count { get; set; }

        /// <summary>
        /// Specifies details on how to format the Variable values.
        /// </summary>
        [Optional] public ValueFormat format { get; set; }
    }

}
