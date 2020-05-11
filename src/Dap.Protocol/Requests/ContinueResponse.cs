using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ContinueResponse
    {
        /// <summary>
        /// If true, the 'continue' request has ignored the specified thread and continued all threads instead.If this attribute is missing a value of 'true' is assumed for backward compatibility.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? AllThreadsContinued { get; set; }
    }

}
