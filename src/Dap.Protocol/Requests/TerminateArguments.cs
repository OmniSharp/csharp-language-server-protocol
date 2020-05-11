using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class TerminateArguments : IRequest<TerminateResponse>
    {
        /// <summary>
        /// A value of true indicates that this 'terminate' request is part of a restart sequence.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? Restart { get; set; }
    }

}
