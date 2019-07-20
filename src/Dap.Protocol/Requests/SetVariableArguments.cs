using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetVariableArguments : IRequest<SetVariableResponse>
    {
        /// <summary>
        /// The reference of the variable container.
        /// </summary>
        public long variablesReference { get; set; }

        /// <summary>
        /// The name of the variable in the container.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Specifies details on how to format the response value.
        /// </summary>
        [Optional] public ValueFormat format { get; set; }
    }

}
