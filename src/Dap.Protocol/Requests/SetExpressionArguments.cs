using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? FrameId { get; set; }

        /// <summary>
        /// Specifies how the resulting value should be formatted.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public ValueFormat Format { get; set; }
    }

}
