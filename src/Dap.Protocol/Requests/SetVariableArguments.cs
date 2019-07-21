using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SetVariableArguments : IRequest<SetVariableResponse>
    {
        /// <summary>
        /// The reference of the variable container.
        /// </summary>
        public long VariablesReference { get; set; }

        /// <summary>
        /// The name of the variable in the container.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Specifies details on how to format the response value.
        /// </summary>
        [Optional] public ValueFormat Format { get; set; }
    }

}
