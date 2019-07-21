using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class EvaluateArguments : IRequest<EvaluateResponse>
    {
        /// <summary>
        /// The expression to evaluate.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Evaluate the expression in the scope of this stack frame. If not specified, the expression is evaluated in the global scope.
        /// </summary>
        [Optional] public long? FrameId { get; set; }

        /// <summary>
        /// The context in which the evaluate request is run.
        /// Values:
        /// 'watch': evaluate is run in a watch.
        /// 'repl': evaluate is run from REPL console.
        /// 'hover': evaluate is run from a data hover.
        /// etc.
        /// </summary>
        [Optional] public string Context { get; set; }

        /// <summary>
        /// Specifies details on how to format the Evaluate result.
        /// </summary>
        [Optional] public ValueFormat Format { get; set; }
    }

}
