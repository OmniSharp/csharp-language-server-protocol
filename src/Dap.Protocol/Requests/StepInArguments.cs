using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class StepInArguments : IRequest<StepInResponse>
    {
        /// <summary>
        /// Execute 'stepIn' for this thread.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// Optional id of the target to step into.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? TargetId { get; set; }
    }

}
