using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetExpressionArguments : IRequest<SetExpressionResponse>
    {
        /// <summary>
        /// The l-value expression to assign to.
        /// </summary>
        public string expression { get; set; }

        /// <summary>
        /// The value expression to assign to the l-value expression.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Evaluate the expressions in the scope of this stack frame. If not specified, the expressions are evaluated in the global scope.
        /// </summary>
        [Optional] public long? frameId { get; set; }

        /// <summary>
        /// Specifies how the resulting value should be formatted.
        /// </summary>
        [Optional] public ValueFormat format { get; set; }
    }

}
