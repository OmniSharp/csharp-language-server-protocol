using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.SetExpression, Direction.ClientToServer)]
    public class SetExpressionArguments : IRequest<SetExpressionResponse>
    {
        /// <summary>
        /// The l-value expression to assign to.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// The value expression to assign to the l-value expression.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Evaluate the expressions in the scope of this stack frame. If not specified, the expressions are evaluated in the global scope.
        /// </summary>
        [Optional] public long? FrameId { get; set; }

        /// <summary>
        /// Specifies how the resulting value should be formatted.
        /// </summary>
        [Optional] public ValueFormat Format { get; set; }
    }

}
