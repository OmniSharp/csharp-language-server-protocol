using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class TerminateThreadsArguments : IRequest<TerminateThreadsResponse>
    {
        /// <summary>
        /// Ids of threads to be terminated.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<long> ThreadIds { get; set; }
    }

}
