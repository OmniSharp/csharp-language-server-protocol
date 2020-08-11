using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class RunInTerminalResponse
    {
        /// <summary>
        /// The process ID.
        /// </summary>
        [Optional]
        public long? ProcessId { get; set; }

        /// <summary>
        /// The process ID of the terminal shell.
        /// </summary>
        [Optional]
        public long? ShellProcessId { get; set; }
    }
}
