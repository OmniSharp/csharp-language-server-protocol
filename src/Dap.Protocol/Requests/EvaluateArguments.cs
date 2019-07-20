using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class EvaluateArguments : IRequest<EvaluateResponse>
    {
        /// <summary>
        /// The expression to evaluate.
        /// </summary>
        public string expression { get; set; }

        /// <summary>
        /// Evaluate the expression in the scope of this stack frame. If not specified, the expression is evaluated in the global scope.
        /// </summary>
        [Optional] public long? frameId { get; set; }

        /// <summary>
        /// The context in which the evaluate request is run.
        /// Values:
        /// 'watch': evaluate is run in a watch.
        /// 'repl': evaluate is run from REPL console.
        /// 'hover': evaluate is run from a data hover.
        /// etc.
        /// </summary>
        [Optional] public string context { get; set; }

        /// <summary>
        /// Specifies details on how to format the Evaluate result.
        /// </summary>
        [Optional] public ValueFormat format { get; set; }
    }

}
