using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ProcessEvent : IRequest
    {

        /// <summary>
        /// The logical name of the process. This is usually the full path to process's executable file. Example: /home/example/myproj/program.js.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The system process id of the debugged process. This property will be missing for non-system processes.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? SystemProcessId { get; set; }

        /// <summary>
        /// If true, the process is running on the same computer as the debug adapter.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public bool? IsLocalProcess { get; set; }

        /// <summary>
        /// Describes how the debug engine started debugging this process.
        /// 'launch': Process was launched under the debugger.
        /// 'attach': Debugger attached to an existing process.
        /// 'attachForSuspendedLaunch': A project launcher component has launched a new process in a suspended state and then asked the debugger to attach.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public ProcessEventStartMethod StartMethod { get; set; }

        /// <summary>
        /// The size of a pointer or address for this process, in bits. This value may be used by clients when formatting addresses for display.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public long? PointerSize { get; set; }
    }

}
