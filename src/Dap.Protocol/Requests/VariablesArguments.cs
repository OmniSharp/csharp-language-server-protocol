using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Variables, Direction.ClientToServer)]
    public class VariablesArguments : IRequest<VariablesResponse>
    {
        /// <summary>
        /// The Variable reference.
        /// </summary>
        public long VariablesReference { get; set; }

        /// <summary>
        /// Optional filter to limit the child variables to either named or indexed.If ommited, both types are fetched.
        /// </summary>
        [Optional] public VariablesArgumentsFilter Filter { get; set; }

        /// <summary>
        /// The index of the first variable to return; if omitted children start at 0.
        /// </summary>
        [Optional] public long? Start { get; set; }

        /// <summary>
        /// The number of variables to return. If count is missing or 0, all variables are returned.
        /// </summary>
        [Optional] public long? Count { get; set; }

        /// <summary>
        /// Specifies details on how to format the Variable values.
        /// </summary>
        [Optional] public ValueFormat Format { get; set; }
    }

}
