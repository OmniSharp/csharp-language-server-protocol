using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class RunInTerminalResponse
    {
        /// <summary>
        /// The process ID.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? ProcessId { get; set; }

        /// <summary>
        /// The process ID of the terminal shell.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? ShellProcessId { get; set; }
    }

}
