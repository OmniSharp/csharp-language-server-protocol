using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class RunInTerminalResponse
    {
        /// <summary>
        /// The process ID.
        /// </summary>
        [Optional] public long? processId { get; set; }

        /// <summary>
        /// The process ID of the terminal shell.
        /// </summary>
        [Optional] public long? shellProcessId { get; set; }
    }

}
