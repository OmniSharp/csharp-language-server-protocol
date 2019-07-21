using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class DataBreakpointInfoArguments : IRequest<DataBreakpointInfoResponse>
    {
        /// <summary>
        /// Reference to the Variable container if the data breakpoint is requested for a child of the container.
        /// </summary>
        [Optional] public long? VariablesReference { get; set; }

        /// <summary>
        /// The name of the Variable's child to obtain data breakpoint information for. If variableReference isn’t provided, this can be an expression.
        /// </summary>
        public string Name { get; set; }
    }

}
